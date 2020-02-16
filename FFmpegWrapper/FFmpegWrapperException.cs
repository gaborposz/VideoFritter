using System;
using System.Runtime.InteropServices;

using FFmpeg.AutoGen;

namespace FFmpegWrapper
{
    public class FFmpegWrapperException : Exception
    {
        private FFmpegWrapperException(int ffmpegErrorCodeIn)
            : base(ConvertErrorCodeToMessage(ffmpegErrorCodeIn))
        {
            FFmpegErrorCode = ffmpegErrorCodeIn;
        }

        public int FFmpegErrorCode { get; private set; }

        public static void ThrowInCaseOfError(int ffmpegErrorCode)
        {
            if (ffmpegErrorCode < 0)
            {
                throw new FFmpegWrapperException(ffmpegErrorCode);
            }
        }

        private static unsafe string ConvertErrorCodeToMessage(int ffmpegErrorCodeIn)
        {
            var bufferSize = 1024;
            var buffer = stackalloc byte[bufferSize];
            ffmpeg.av_strerror(ffmpegErrorCodeIn, buffer, (ulong)bufferSize);
            var message = Marshal.PtrToStringAnsi((IntPtr)buffer);
            return $"{message} Error code: 0x{ffmpegErrorCodeIn:X}";
        }
    }
}
