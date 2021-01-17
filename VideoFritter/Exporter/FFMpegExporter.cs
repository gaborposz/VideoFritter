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
                string targetDirectory = Path.GetDirectoryName(targetFileName);
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                using (InputMediaFile inputFile = new InputMediaFile(sourceFileName))
                {
                    // Filter out those streams which codec is not known
                    List<MediaStream> openStreams = inputFile.Streams
                        .Where(s =>
                            s.CodecType == MediaType.AVMEDIA_TYPE_AUDIO ||
                            s.CodecType == MediaType.AVMEDIA_TYPE_VIDEO ||
                            s.CodecType == MediaType.AVMEDIA_TYPE_SUBTITLE)
                        .ToList();

                    inputFile.Seek(sliceStart);

                    using (OutputMediaFile outputFile = new OutputMediaFile(targetFileName))
                    {
                        outputFile.Streams = openStreams.ToArray();

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

                        // Presentation time stamp of the first frame to copy, 
                        // i.e. the "zero" time stamp of the new video file, 
                        // that will be subtracted from all packets' time stamp.
                        // It is needed because Seek will actually 
                        // not seek to the given position, 
                        // but to the first I-Frame before that.
                        TimeSpan? firstFrameTimeStamp = null;

                        // Workaround for the case if an "open stream" 
                        // does not contain packets after the end of the slice
                        TimeSpan emergencyExitTimeStamp = sliceEnd.Add(TimeSpan.FromSeconds(30));

                        while (inputFile.TryRead(out MediaPacket packet))
                        {
                            if (openStreams.Contains(packet.Stream))
                            {
                                if (!firstFrameTimeStamp.HasValue)
                                {
                                    firstFrameTimeStamp = packet.StartTime;
                                }

                                packet.ShiftTime(-firstFrameTimeStamp.Value);

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
                                    TimeSpan totalSliceLength = sliceEnd.Subtract(sliceStart);
                                    TimeSpan currectPositionInSlice = packet.StartTime.Subtract(firstFrameTimeStamp.Value);
                                    progressHandler.Report((double)currectPositionInSlice.Ticks / totalSliceLength.Ticks);
                                }
                            }
                            else
                            {
                                // Stop reading packets if we are (in one stream) already way beyond the end of the slice.
                                // With this we can avoid (the time consuming) reading of the whole file 
                                // if an "open stream" does not contain any packets after the end of the slice.
                                if (packet.StartTime > emergencyExitTimeStamp)
                                {
                                    break;
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
