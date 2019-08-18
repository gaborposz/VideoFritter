using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal class PlayUntilSelectionEndCommand : AbstractOpenedFileEnabledCommand
    {
        public PlayUntilSelectionEndCommand(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn)
            : base(mainWindowViewModelIn, videoPlayerIn)
        {
            VideoPlayer.VideoPositionChanged += VideoPlayer_VideoPositionChanged;
        }

        public override bool CanExecute(object parameter)
        {
            return base.CanExecute(parameter) && 
                VideoPlayer.VideoPosition < MainWindowViewModel.SliceEnd;
        }

        public override void Execute(object parameter)
        {
            VideoPlayer.Play(VideoPlayer.VideoPosition, MainWindowViewModel.SliceEnd);
        }

        private void VideoPlayer_VideoPositionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            SendCanExecuteChanged();
        }
    }
}
