using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace VideoFritter.Exporter
{
    internal class FFMpegExporter
    {
        public string GenerateFileName(string originalFileName, string targetPath)
        {
            int counter = 1;
            string exportedVideoFileName;

            do
            {
                exportedVideoFileName = Path.Combine(
                    targetPath,
                    $"{Path.GetFileNameWithoutExtension(originalFileName)}_{counter}{Path.GetExtension(originalFileName)}");
                counter++;
            } while (File.Exists(exportedVideoFileName));

            return exportedVideoFileName;
        }

        public Task ExportAsync(string sourceFileName, string targetFileName, TimeSpan sliceStart, TimeSpan sliceEnd, CancellationToken cancellationToken, IProgress<double> progressHandler)
        {
            if (string.IsNullOrWhiteSpace(sourceFileName))
            {
                throw new ArgumentNullException(nameof(sourceFileName), "The source file name cannot be null or empty!");
            }

            if (string.IsNullOrWhiteSpace(targetFileName))
            {
                throw new ArgumentNullException(nameof(sourceFileName), "The target file name cannot be null or empty!");
            }


            return Task.Run(() =>
            {
                string targetDirectory = Path.GetDirectoryName(targetFileName);
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                string ffmpegPath = Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg", "ffmpeg.exe");

                string progressFile = Path.GetTempFileName();

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = $"-noaccurate_seek -i {sourceFileName} -ss {sliceStart} -to {sliceEnd} -y -vcodec copy -acodec copy -report -progress {progressFile} {targetFileName}",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };

                Process ffmpegProc = new Process
                {
                    StartInfo = startInfo
                };

                double exportLengthInMs = sliceEnd.Subtract(sliceStart).TotalMilliseconds;

                bool exportFinished = false;
                Task progressWatcherTask;

                if (progressHandler != null)
                {
                    progressWatcherTask = Task.Run(() =>
                    {
                        using (StreamReader streamReader =
                                    new StreamReader(new FileStream(progressFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                        {
                            while (!exportFinished)
                            {
                                while (!streamReader.EndOfStream)
                                {
                                    string line = streamReader.ReadLine();

                                    if (line.StartsWith("out_time="))
                                    {
                                        TimeSpan actualTimeStamp = TimeSpan.Parse(line.Substring(9));
                                        double progress = actualTimeStamp.TotalMilliseconds / exportLengthInMs;
                                        progressHandler.Report(progress);
                                    }
                                }
                                Thread.Sleep(100);
                            }
                        }
                        progressHandler.Report(1);
                    });
                }
                else
                {
                    progressWatcherTask = Task.CompletedTask;
                }


                ffmpegProc.Start();
                ffmpegProc.BeginOutputReadLine();
                ffmpegProc.BeginErrorReadLine();

                ffmpegProc.WaitForExit();

                exportFinished = true;
                progressWatcherTask.Wait();

                File.Delete(progressFile);

                if (ffmpegProc.ExitCode != 0)
                {
                    throw new InvalidOperationException("Export failed:\n" + ffmpegProc.StandardError.ReadToEnd());
                }


            }, cancellationToken);
        }
    }
}
