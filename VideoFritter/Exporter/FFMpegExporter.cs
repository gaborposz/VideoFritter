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

                    inputFile.Seek(sliceStart);

                    using (OutputMediaFile outputFile = new OutputMediaFile(targetFileName))
                    {
                        outputFile.Streams = inputFile.Streams;

                        IDictionary<string, string> metaData = inputFile.MetaData;

                        if (ApplicationSettings.TimeStampCorrection)
                        {
                            DateTime creationTime;
                            if (metaData.TryGetValue(CreationTimeMetadataKey, out string creationTimeString))
                            {
                                creationTime = DateTime.Parse(creationTimeString);
                            }
                            else
                            {
                                // Use modification date as fallback
                                creationTime = File.GetLastWriteTimeUtc(sourceFileName);
                            }
                            DateTime compensatedCreationTime = creationTime.Add(sliceStart);
                            string compensatedCreationTimeString = compensatedCreationTime.ToUniversalTime().ToString("O");
                            metaData[CreationTimeMetadataKey] = compensatedCreationTimeString;
                        }

                        outputFile.MetaData = metaData;

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

        private static readonly string CreationTimeMetadataKey = "creation_time";
    }
}
