using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FFmpegWrapper;

namespace VideoFritter.Exporter
{
    internal class FFMpegExporter
    {
        static FFMpegExporter()
        {
            FFmpegHelper.FFmpegPath = Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg");
        }

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
                //TODO: Get rid of ApplicationSettings.SaveFFMpegLogs

                string targetDirectory = Path.GetDirectoryName(targetFileName);
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                using (InputMediaFile inputFile = new InputMediaFile(sourceFileName))
                {
                    List<MediaStream> openStreams = inputFile.Streams.ToList();

                    if (ApplicationSettings.TimeStampCorrection)
                    {
                        DateTime creationTime = GetCreationTimeFromFile(sourceFileName);
                        creationTime = creationTime.Add(sliceStart);
                        //TODO: Metadata copy and correction
                    }

                    inputFile.Seek(sliceStart);

                    using (OutputMediaFile outputFile = new OutputMediaFile(targetFileName))
                    {
                        outputFile.Streams = inputFile.Streams;

                        outputFile.WriteHeader();
                        while (inputFile.TryRead(out MediaPacket packet))
                        {
                            if (openStreams.Contains(packet.Stream))
                            {
                                outputFile.WritePacket(packet);

                                if (packet.EndTime > sliceEnd && packet.KeyFrame)
                                {
                                    openStreams.Remove(packet.Stream);
                                    if (openStreams.Count == 0)
                                    {
                                        break;
                                    }
                                }

                                if (progressHandler != null)
                                {
                                    TimeSpan totalLength = sliceEnd.Subtract(sliceStart);
                                    progressHandler.Report((double)packet.EndTime.Ticks / totalLength.Ticks);
                                }
                            }
                        }
                        outputFile.WriteTailer();
                        if (progressHandler != null)
                        {
                            progressHandler.Report(1);
                        }
                    }
                }

                if (ApplicationSettings.TimeStampCorrection)
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
                throw new ExporterException($"{startInfo.FileName} {arguments}", errorOutput.Result);
            }
        }

        private DateTime GetCreationTimeFromFile(string fileName)
        {
            // TODO: Make it using ffmpeg library directly too

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
