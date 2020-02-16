using FFmpeg.AutoGen;

namespace VideoFritter.FFmpegWrapper
{
    public class FFmpegFrameRate : AbstractRationalNumber
    {
        internal FFmpegFrameRate(AVRational avRationalIn)
            : base(avRationalIn)
        {
        }

        public double Value
        {
            get
            {
                return (double)Numerator / Denominator;
            }
        }

        public override string ToString()
        {
            return $"{(double)Numerator / Denominator:F2} fps";
        }
    }
}
