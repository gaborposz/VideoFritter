using System;
using System.Windows;

namespace VideoFritter.VideoPlayer
{
    public class VideoPositionChangedEventArgs : RoutedEventArgs
    {
        public VideoPositionChangedEventArgs(RoutedEvent routedEvent, object source, TimeSpan newPosition)
            : base(routedEvent, source)
        {
            NewPosition = newPosition;
        }

        public TimeSpan NewPosition { get; }
    }
}
