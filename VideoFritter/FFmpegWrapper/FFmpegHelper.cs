using FFmpeg.AutoGen;

namespace VideoFritter.FFmpegWrapper
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
