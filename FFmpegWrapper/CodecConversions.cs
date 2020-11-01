
using FFmpeg.AutoGen;

namespace FFmpegWrapper
{
    internal static class EnumConversions
    {
        public static CodecId ConvertToCodecId(this AVCodecID codecId)
        {
            return (CodecId)codecId;
        }

        public static MediaType ConvertToMediaType(this AVMediaType mediaType)
        {
            return (MediaType)mediaType;
        }
    }
}
