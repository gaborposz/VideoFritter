using FFmpeg.AutoGen;

namespace FFmpegWrapper
{
    public abstract class AbstractRationalNumber
    {
        internal AbstractRationalNumber(AVRational avRationalIn)
        {
            RationalNumber = avRationalIn;
        }

        public override string ToString()
        {
            return $"{Numerator}/{Denominator}";
        }

        internal AVRational RationalNumber { get; private set; }

        protected int Numerator => RationalNumber.num;

        protected int Denominator => RationalNumber.den;

    }
}
