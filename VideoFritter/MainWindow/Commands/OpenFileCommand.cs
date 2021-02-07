
using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal class OpenFileCommand : AbstractMainWindowCommandBase
    {
        public OpenFileCommand(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn)
            : base(mainWindowViewModelIn)
        {
            VideoPlayer = videoPlayerIn;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            if (MainWindowViewModel.OpenFile())
            {
                VideoPlayer.OpenFile(MainWindowViewModel.OpenedFileName);
            }
        }

        private VideoPlayer VideoPlayer { get; }
    }
}
