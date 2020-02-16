using System;

using FFmpeg.AutoGen;

namespace FFmpegWrapper
{
    internal static class FFmpegExtensions
    {
        public static double ConvertToFps(this AVRational avRational)
        {
            return (double)avRational.num / avRational.den;
        }

        public static TimeSpan ConvertToTimeSpan(this long value, AVRational baseUnit)
        {
            return TimeSpan.FromSeconds((double)value * baseUnit.num / baseUnit.den);
        }

        public static long ConvertToDuration(this TimeSpan value, AVRational baseUnit)
        {
            return (long)(value.TotalSeconds * baseUnit.den / baseUnit.num);
        }
    }
}
