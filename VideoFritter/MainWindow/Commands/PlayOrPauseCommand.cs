
using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal class PlayOrPauseCommand : AbstractOpenedFileEnabledCommand
    {
        public PlayOrPauseCommand(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn)
            : base(mainWindowViewModelIn, videoPlayerIn)
        {
            VideoPlayer.VideoPositionChanged += VideoPlayer_VideoPositionChanged;
        }

        public override bool CanExecute(object parameter)
        {
            return base.CanExecute(parameter) && 
                VideoPlayer.VideoPosition < VideoPlayer.VideoLength;
        }

        public override void Execute(object parameter)
        {
            VideoPlayer.PlayOrPause();
        }

        private void VideoPlayer_VideoPositionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            SendCanExecuteChanged();
        }
    }
}
