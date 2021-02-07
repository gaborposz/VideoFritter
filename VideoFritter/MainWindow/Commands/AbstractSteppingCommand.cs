using System;

using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal abstract class AbstractSteppingCommand : AbstractOpenedFileEnabledCommand
    {
        public AbstractSteppingCommand(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn)
            : base(mainWindowViewModelIn, videoPlayerIn)
        {
            VideoPlayer.IsPlayingChanged += VideoPlayer_IsPlayingChanged;
            VideoPlayer.VideoPositionChanged += VideoPlayer_VideoPositionChanged;
        }

        private void VideoPlayer_VideoPositionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            SendCanExecuteChanged();
        }

        private void VideoPlayer_IsPlayingChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            SendCanExecuteChanged();
        }
    }
}
