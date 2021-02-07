using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal class PlayUntilSelectionEndCommand : AbstractOpenedFileEnabledCommand
    {
        public PlayUntilSelectionEndCommand(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn)
            : base(mainWindowViewModelIn)
        {
            VideoPlayer = videoPlayerIn;

            VideoPlayer.VideoPositionChanged += VideoPlayer_VideoPositionChanged;
            MainWindowViewModel.PropertyChanged += MainWindowViewModel_PropertyChanged;
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

        private VideoPlayer VideoPlayer { get; }

        private void VideoPlayer_VideoPositionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            SendCanExecuteChanged();
        }

        private void MainWindowViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.SliceEnd))
            {
                SendCanExecuteChanged();
            }
        }
    }
}
