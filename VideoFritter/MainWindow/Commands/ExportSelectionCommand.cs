using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal class ExportSelectionCommand : AbstractOpenedFileEnabledCommand
    {
        public ExportSelectionCommand(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn)
            : base(mainWindowViewModelIn, videoPlayerIn)
        {
            MainWindowViewModel.IsExportingChangedEvent += MainWindowViewModel_IsExportingChangedEvent;
        }

        public override bool CanExecute(object parameter)
        {
            return base.CanExecute(parameter) && !MainWindowViewModel.IsExporting;
        }

        public override void Execute(object parameter)
        {
            MainWindowViewModel.ExportCurrentSelection();
        }

        private void MainWindowViewModel_IsExportingChangedEvent(object sender, bool e)
        {
            SendCanExecuteChanged();
        }
    }
}
