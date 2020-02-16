using System;

using FFmpeg.AutoGen;

namespace VideoFritter.FFmpegWrapper
{
    public unsafe class MediaPacket : IDisposable
    {
        internal MediaPacket(AVPacket* packetPtrIn, MediaStream streamIn)
        {
            PacketPtr = packetPtrIn;
            Stream = streamIn;
            StartTime = Stream.TimeBase.ToTimeSpan(packetPtrIn->dts);
            EndTime = Stream.TimeBase.ToTimeSpan(packetPtrIn->dts + packetPtrIn->duration);
            KeyFrame = (packetPtrIn->flags & ffmpeg.AV_PKT_FLAG_KEY) == ffmpeg.AV_PKT_FLAG_KEY;
        }

        ~MediaPacket()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public MediaStream Stream { get; private set; }

        public TimeSpan StartTime { get; private set; }

        public TimeSpan EndTime { get; private set; }

        public bool KeyFrame { get; private set; }

        internal AVPacket* PacketPtr { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // dispose managed objects (if there is any).
                }

                ffmpeg.av_packet_unref(PacketPtr);
                ffmpeg.av_free(PacketPtr);

                this.disposed = true;
            }
        }

        private bool disposed;

    }
}
