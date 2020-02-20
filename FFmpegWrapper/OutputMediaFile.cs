using System;
using System.Collections.Generic;
using FFmpeg.AutoGen;

namespace FFmpegWrapper
{
    public unsafe class OutputMediaFile : IDisposable
    {
        public OutputMediaFile(string fileNameIn)
        {
            try
            {
                fixed (AVIOContext** avioContextPtrPtr = &this.avioContextPtr)
                {
                    int error = ffmpeg.avio_open(avioContextPtrPtr, fileNameIn, ffmpeg.AVIO_FLAG_WRITE);
                    FFmpegWrapperException.ThrowInCaseOfError(error);
                }

                fixed (AVFormatContext** formatContextPtrPtr = &this.avFormatContextPtr)
                {
                    int error = ffmpeg.avformat_alloc_output_context2(formatContextPtrPtr, null, null, fileNameIn);
                    FFmpegWrapperException.ThrowInCaseOfError(error);
                    this.avFormatContextPtr->pb = avioContextPtr;
                }
            }
            catch
            {
                ReleaseTheUnmanagedResources();
                throw;
            }
        }

        public MediaStream[] Streams
        {
            get
            {
                return this.streams;
            }
            set
            {
                foreach (MediaStream stream in value)
                {
                    AVStream* newStream = ffmpeg.avformat_new_stream(this.avFormatContextPtr, null);
                    fixed (AVCodecParameters* codecParametersPtr = &stream.CodecParameters)
                    {
                        ffmpeg.avcodec_parameters_copy(newStream->codecpar, codecParametersPtr);
                    }
                    newStream->time_base = stream.TimeBase.RationalNumber;
                }

                this.streams = value;
            }
        }

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
            set
            {
                ffmpeg.av_dict_free(&this.avFormatContextPtr->metadata);
                foreach (KeyValuePair<string, string> item in value)
                {
                    int error = ffmpeg.av_dict_set(&this.avFormatContextPtr->metadata, item.Key, item.Value, 0);
                    FFmpegWrapperException.ThrowInCaseOfError(error);
                }
            }
        }


        public void WriteHeader()
        {
            if (Streams == null)
            {
                throw new InvalidOperationException("Streams were not added yet!");
            }

            int error = ffmpeg.avformat_write_header(this.avFormatContextPtr, null);
            FFmpegWrapperException.ThrowInCaseOfError(error);
        }

        public void WritePacket(MediaPacket packetIn)
        {
            int error = ffmpeg.av_write_frame(this.avFormatContextPtr, packetIn.PacketPtr);
            FFmpegWrapperException.ThrowInCaseOfError(error);
        }

        public void WriteTailer()
        {
            int error = ffmpeg.av_write_trailer(this.avFormatContextPtr);
            FFmpegWrapperException.ThrowInCaseOfError(error);
        }

        ~OutputMediaFile()
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

                ReleaseTheUnmanagedResources();

                this.disposed = true;
            }
        }

        private AVIOContext* avioContextPtr;
        private AVFormatContext* avFormatContextPtr;
        private bool disposed = false;
        private MediaStream[] streams;

        private void ReleaseTheUnmanagedResources()
        {
            if (this.avioContextPtr != null)
            {
                ffmpeg.avformat_free_context(this.avFormatContextPtr);
                this.avFormatContextPtr = null;
            }

            if (this.avioContextPtr != null)
            {
                ffmpeg.avio_close(avioContextPtr);
                this.avioContextPtr = null;
            }
        }
    }
}
