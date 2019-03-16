using System;
using System.Collections.Generic;
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

                string timeStampFixCmdLine = string.Empty;

                if (Properties.Settings.Default.TimeStampCorrection)
                {
                    DateTime creationTime = GetCreationTimeFromFile(sourceFileName);
                    creationTime = creationTime.Add(sliceStart);
                    timeStampFixCmdLine = $"-metadata creation_time=\"{creationTime.ToString("yyyy-MM-dd HH:mm:ss")}\"";
                }

                string progressFile = Path.GetTempFileName();
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

                try
                {
                    ExecuteFFmpegProcess(
                        $"-noaccurate_seek -i {sourceFileName} -ss {sliceStart} -to {sliceEnd} {timeStampFixCmdLine} -map_metadata 0 -movflags use_metadata_tags -y -vcodec copy -acodec copy -report -progress {progressFile} {targetFileName}");
                }
                finally
                {
                    exportFinished = true;
                    progressWatcherTask.Wait();
                    File.Delete(progressFile);
                }

                if (Properties.Settings.Default.TimeStampCorrection)
                {
                    // Also restore the file modification date...
                    DateTime fileModificationTime = File.GetLastWriteTime(sourceFileName);
                    fileModificationTime = fileModificationTime.Add(sliceStart);
                    File.SetLastWriteTime(targetFileName, fileModificationTime);
                }
            }, cancellationToken);
        }

        private void ExecuteFFmpegProcess(string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg", "ffmpeg.exe"),
                Arguments = arguments,
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

            ffmpegProc.Start();
            ffmpegProc.BeginOutputReadLine();

            Task<string> errorOutput = ffmpegProc.StandardError.ReadToEndAsync();

            ffmpegProc.WaitForExit();

            if (ffmpegProc.ExitCode != 0)
            {
                throw new InvalidOperationException($"Export failed!\nCommand line: ffmpeg {arguments}\nError: {errorOutput.Result}");
            }
        }

        private DateTime GetCreationTimeFromFile(string fileName)
        {
            string metadataFileName = Path.GetTempFileName();
            try
            {
                ExecuteFFmpegProcess($"-i {fileName} -map_metadata 0 -y -f ffmetadata {metadataFileName}");
                IEnumerable<string> metadataFile = File.ReadLines(metadataFileName);
                foreach (string line in metadataFile)
                {
                    if (line.StartsWith("creation_time="))
                    {
                        string creationTimeText = line.Split('=')[1];
                        if (DateTime.TryParse(creationTimeText, out DateTime creationTime))
                        {
                            return creationTime;
                        }
                        break;
                    }
                }
            }
            finally
            {
                File.Delete(metadataFileName);
            }

            // Use modification date as fallback
            return File.GetLastWriteTimeUtc(fileName);
        }
    }
}
