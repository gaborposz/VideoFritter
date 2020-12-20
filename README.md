# VideoFritter
VideoFritter is a free and open source .NET Core 3.1 based WPF Application that provides a convenient UI over ffmpeg for exporting shorter clips from videos without quality loss. It is especially useful for preprocessing large video files, e.g. getting rid of boring parts or camera shaking at the end or the beginning.

The playback is implemented via the WPF MediaElement control (i.e. a Windows Media Player), therefore it uses HW acceleration (if  supported by the OS). The export functionaly is using ffmpeg in lossless mode, therefore it's quite fast since it does not encode, but just copies files.

VideoFritter supports video formats, resolutions and codecs which are supported by both ffmpeg and Windows Media Player, e.g.: avi, mov, mp4, H264, H265, FullHD, 4k, etc.

The latest Release can be downloaded from the [Releases](https://github.com/gaborposz/VideoFritter/releases) page.

If you want to provide feedback then please use the [Issues](https://github.com/gaborposz/VideoFritter/issues) tab on the top.
