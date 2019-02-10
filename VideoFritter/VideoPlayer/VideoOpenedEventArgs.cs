using System;
using System.Windows;

namespace VideoFritter.VideoPlayer
{
    public class VideoOpenedEventArgs : RoutedEventArgs
    {
        public VideoOpenedEventArgs(RoutedEvent routedEvent, object source, int videoWidth, int videoHeight)
            : base(routedEvent, source)
        {
            VideoWidth = videoWidth;
            VideoHeight = videoHeight;
        }

        public int VideoWidth { get; }
        public int VideoHeight { get; }
    }
}
