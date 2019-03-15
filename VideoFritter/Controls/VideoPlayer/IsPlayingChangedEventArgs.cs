using System.Windows;

namespace VideoFritter.Controls.VideoPlayer
{
    public class IsPlayingChangedEventArgs : RoutedEventArgs
    {
        public IsPlayingChangedEventArgs(RoutedEvent routedEvent, object source, bool newIsPlaying)
            : base(routedEvent, source)
        {
            NewIsPlaying = newIsPlaying;
        }

        public bool NewIsPlaying { get; }
    }
}
