using System.Windows;

using VideoFritter.ExportQueue;

namespace VideoFritter.MainWindow.Commands
{
    internal class OpenExportQueueCommand : AbstractOpenDialogCommand
    {
        public OpenExportQueueCommand(MainWindowViewModel mainWindowViewModelIn, ExportQueueViewModel exportQueueViewModelIn)
            : base(mainWindowViewModelIn)
        {
            ExportQueueViewModel = exportQueueViewModelIn;
        }


        protected override Window CreateDialog()
        {
            return new ExportQueueWindow { DataContext = ExportQueueViewModel };
        }

        private ExportQueueViewModel ExportQueueViewModel { get; }
    }
}
