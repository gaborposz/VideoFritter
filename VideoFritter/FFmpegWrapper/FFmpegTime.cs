﻿using System;

using FFmpeg.AutoGen;

namespace VideoFritter.FFmpegWrapper
{
    public class FFmpegTime : AbstractRationalNumber
    {

        internal FFmpegTime(AVRational avRationalIn)
            : base(avRationalIn)
        {
        }

        public override string ToString()
        {
            double milliSeconds = 1000d * Numerator / Denominator;
            return $"{base.ToString()} s ({milliSeconds:F2} ms)";
        }

        public TimeSpan ToTimeSpan(long multiplier)
        {
            return TimeSpan.FromSeconds((double)multiplier * Numerator / Denominator);
        }
    }
}
