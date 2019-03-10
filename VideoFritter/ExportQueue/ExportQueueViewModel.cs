using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using VideoFritter.Common;
using VideoFritter.Exporter;

namespace VideoFritter.ExportQueue
{
    internal class ExportQueueViewModel : AbstractExportingViewModel
    {
        public ExportQueueViewModel()
        {
            Queue = new ObservableCollection<ExportItem>();
        }

        public ObservableCollection<ExportItem> Queue
        {
            get;
        }

        public void AddToQueue(string fileName, TimeSpan sliceStart, TimeSpan sliceEnd)
        {
            Queue.Add(new ExportItem(fileName, sliceStart, sliceEnd));
        }

        public void ExportQueue()
        {
            if (Queue.Count > 0)
            {
                IsExporting = true;
                ExportItem processingItem = Queue[0];
                string targetDirectory = Path.Combine(Path.GetDirectoryName(processingItem.FileName), "TEMP");
                string targetFileName = this.exporter.GenerateFileName(processingItem.FileName, targetDirectory);
                Task exportTask = this.exporter.ExportAsync(processingItem.FileName, targetFileName, processingItem.SliceStart, processingItem.SliceEnd, CancellationToken.None, this);
                exportTask.ContinueWith((task) =>
                {
                    Queue.Remove(processingItem);
                    ExportProgress = 0;
                    ExportQueue(); // Take the next item
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                IsExporting = false;
            }
        }

        private readonly FFMpegExporter exporter = new FFMpegExporter();
    }
}
