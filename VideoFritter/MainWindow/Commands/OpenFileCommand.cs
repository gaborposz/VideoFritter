
using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal class OpenFileCommand : AbstractMainWindowCommandBase
    {
        public OpenFileCommand(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn)
            : base(mainWindowViewModelIn, videoPlayerIn)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            MainWindowViewModel.OpenFile();
            if (MainWindowViewModel.IsFileOpened)
            {
                VideoPlayer.OpenFile(MainWindowViewModel.OpenedFileName);
            }
        }
    }
}
