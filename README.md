# VideoFritter
VideoFritter is a free and open source .NET based WPF Application that provides a convenient UI over ffmpeg for exporting shorter clips from videos without quality loss. It is especially useful for preprocessing large raw video files, e.g. clipping out only the remarkable parts of a clip.

The playback is implemented via the WPF MediaElement control (i.e. a Windows Media Player), therefore it uses HW acceleration (if  supported by the OS). The export functionaly is using ffmpeg in lossless mode, therefore it's quite fast since it does not encode, but just copies files.

VideoFritter supports various video formats, resolutions and codecs which are supported by both ffmpeg and Windows Media Player, e.g.: avi, mov, mp4, H264, H265, FullHD, 4k, etc.
