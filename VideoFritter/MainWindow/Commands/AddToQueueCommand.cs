using System;

using VideoFritter.ExportQueue;

namespace VideoFritter.MainWindow.Commands
{
    internal class AddToQueueCommand : AbstractOpenedFileEnabledCommand
    {
        public AddToQueueCommand(MainWindowViewModel mainWindowViewModelIn, ExportQueueViewModel exportQueueViewModelIn)
            : base(mainWindowViewModelIn)
        {
            ExportQueueViewModel = exportQueueViewModelIn;
        }

        public override void Execute(object parameter)
        {
            ExportQueueViewModel.AddToQueue(MainWindowViewModel.OpenedFileName, MainWindowViewModel.SliceStart, MainWindowViewModel.SliceEnd);
        }

        private ExportQueueViewModel ExportQueueViewModel { get; }
    }
}
