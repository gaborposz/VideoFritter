using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal abstract class AbstractOpenedFileEnabledCommand : AbstractMainWindowCommandBase
    {
        public AbstractOpenedFileEnabledCommand(MainWindowViewModel mainWindowViewModelIn)
            : base(mainWindowViewModelIn)
        {
            MainWindowViewModel.IsFileOpenedChanged += MainWindowViewModelIsFileOpenedChangedHandler;
        }

        public override bool CanExecute(object parameter)
        {
            return MainWindowViewModel.IsFileOpened;
        }

        private void MainWindowViewModelIsFileOpenedChangedHandler(object sender, bool e)
        {
            SendCanExecuteChanged();
        }
    }
}
