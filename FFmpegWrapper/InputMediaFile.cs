using System;
using System.Collections.Generic;

using FFmpeg.AutoGen;

namespace FFmpegWrapper
{
    public unsafe class InputMediaFile : IDisposable
    {
        public InputMediaFile(string filePath)
        {
            // TODO: File exist check
            int errorCode;

            this.avFormatContextPtr = ffmpeg.avformat_alloc_context();

            fixed (AVFormatContext** formatContextPointerPointer = &this.avFormatContextPtr)
            {
                errorCode = ffmpeg.avformat_open_input(formatContextPointerPointer, filePath, null, null);
                FFmpegWrapperException.ThrowInCaseOfError(errorCode);
            }

            errorCode = ffmpeg.avformat_find_stream_info(this.avFormatContextPtr, null);
            FFmpegWrapperException.ThrowInCaseOfError(errorCode);

            Streams = CreateStreams();
        }

        public MediaStream[] Streams { get; private set; }

        public IDictionary<string, string> MetaData
        {
            get
            {
                if (this.avFormatContextPtr->metadata == null)
                {
                    return null;
                }

                return FFmpegHelper.DictionaryConvert(this.avFormatContextPtr->metadata);
            }
        }

        public void Seek(TimeSpan seekToIn)
        {
            long positionInBaseUnits = (long)(seekToIn.TotalSeconds * ffmpeg.AV_TIME_BASE);
            int errorCode = ffmpeg.av_seek_frame(this.avFormatContextPtr, -1, positionInBaseUnits, ffmpeg.AVSEEK_FLAG_BACKWARD);
            FFmpegWrapperException.ThrowInCaseOfError(errorCode);
        }

        public bool TryRead(out MediaPacket mediaPacketOut)
        {
            AVPacket* packetPtr = ffmpeg.av_packet_alloc();
            int errorCode = ffmpeg.av_read_frame(this.avFormatContextPtr, packetPtr);
            if (errorCode == ffmpeg.AVERROR_EOF)
            {
                ffmpeg.av_packet_unref(packetPtr);
                ffmpeg.av_free(packetPtr);
                mediaPacketOut = null;
                return false;
            }

            try
            {
                FFmpegWrapperException.ThrowInCaseOfError(errorCode);
            }
            catch
            {
                ffmpeg.av_packet_unref(packetPtr);
                ffmpeg.av_free(packetPtr);
                throw;
            }

            mediaPacketOut = new MediaPacket(packetPtr, Streams[packetPtr->stream_index]);

            return true;
        }

        ~InputMediaFile()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // dispose managed objects (if there is any).
                }

                fixed (AVFormatContext** formatContextPtrPtr = &this.avFormatContextPtr)
                {
                    ffmpeg.avformat_close_input(formatContextPtrPtr);
                }

                this.disposed = true;
            }
        }

        private readonly AVFormatContext* avFormatContextPtr;

        private bool disposed = false;

        private MediaStream[] CreateStreams()
        {
            MediaStream[] mediaStreams = new MediaStream[this.avFormatContextPtr->nb_streams];
            AVStream** streamsPointer = this.avFormatContextPtr->streams;
            for (int i = 0; i < mediaStreams.Length; i++)
            {
                mediaStreams[i] = new MediaStream(*streamsPointer);
                streamsPointer++;
            }

            return mediaStreams;
        }

    }
}
