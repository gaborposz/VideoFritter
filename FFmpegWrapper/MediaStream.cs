using FFmpeg.AutoGen;

namespace FFmpegWrapper
{
    public unsafe class MediaStream
    {
        internal MediaStream(AVStream* avStreamPtrIn)
        {
            StreamIndex = avStreamPtrIn->index;
            TimeBase = new FFmpegTime(avStreamPtrIn->time_base);
            FrameRate = new FFmpegFrameRate(avStreamPtrIn->r_frame_rate);
            NumberOfFrames = avStreamPtrIn->nb_frames;
            Codec = avStreamPtrIn->codec->codec_id.ToString();
            CodecType = avStreamPtrIn->codec->codec_type.ToString();
            CodecParameters = *avStreamPtrIn->codecpar;
            Width = avStreamPtrIn->codec->width;
            Height = avStreamPtrIn->codec->height;
        }

        public int StreamIndex { get; private set; }

        public FFmpegTime TimeBase { get; private set; }

        public FFmpegFrameRate FrameRate { get; private set; }

        public long NumberOfFrames { get; private set; }

        //TODO: Use specific enum instead of string
        public string Codec { get; private set; }

        //TODO: Use specific enum instead of string
        public string CodecType { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        internal AVCodecParameters CodecParameters;

    }
}
