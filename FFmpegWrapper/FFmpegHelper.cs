using FFmpeg.AutoGen;

namespace FFmpegWrapper
{
    public static class FFmpegHelper
    {
        public static string FFmpegPath
        {
            set
            {
                ffmpeg.RootPath = value;
            }
        }

    }
}
