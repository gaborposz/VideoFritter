using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal class PlaySelectionCommand : AbstractOpenedFileEnabledCommand
    {
        public PlaySelectionCommand(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn)
            : base(mainWindowViewModelIn)
        {
            VideoPlayer = videoPlayerIn;
        }

        public override void Execute(object parameter)
        {
            VideoPlayer.Play(MainWindowViewModel.SliceStart, MainWindowViewModel.SliceEnd);
        }

        private VideoPlayer VideoPlayer { get; }
    }
}
