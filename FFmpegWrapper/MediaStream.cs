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
            Codec = avStreamPtrIn->codecpar->codec_id.ConvertToCodecId();
            CodecType = avStreamPtrIn->codecpar->codec_type.ConvertToMediaType();
            CodecParameters = *avStreamPtrIn->codecpar;
            Width = avStreamPtrIn->codecpar->width;
            Height = avStreamPtrIn->codecpar->height;
        }

        public int StreamIndex { get; private set; }

        public FFmpegTime TimeBase { get; private set; }

        public FFmpegFrameRate FrameRate { get; private set; }

        public long NumberOfFrames { get; private set; }

        public CodecId Codec { get; private set; }

        public MediaType CodecType { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        internal AVCodecParameters CodecParameters;

    }
}
