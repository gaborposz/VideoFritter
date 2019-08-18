using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal class PlayFromSelectionStartCommand : AbstractOpenedFileEnabledCommand
    {
        public PlayFromSelectionStartCommand(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn)
            : base(mainWindowViewModelIn, videoPlayerIn)
        {
        }

        public override void Execute(object parameter)
        {
            VideoPlayer.Play(MainWindowViewModel.SliceStart, VideoPlayer.VideoLength);
        }
    }
}
