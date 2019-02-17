using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using VideoFritter.Common;
using VideoFritter.Exporter;

namespace VideoFritter.ProcessingQueue
{
    internal class ProcessingQueueViewModel : AbstractViewModelBase
    {
        public ProcessingQueueViewModel()
        {
            Queue = new ObservableCollection<ProcessingItem>();
        }

        public ObservableCollection<ProcessingItem> Queue
        {
            get;
        }

        public void AddToQueue(string fileName, TimeSpan sliceStart, TimeSpan sliceEnd)
        {
            lock (this.queueLock)
            {
                Queue.Add(new ProcessingItem(fileName, sliceStart, sliceEnd));
            }
        }

        public void ExportQueue()
        {
            /*Task.Run(() =>
            {*/
                FFMpegExporter exporter = new FFMpegExporter();

                while (TryGetNextItem(out ProcessingItem processingItem))
                {
                    string targetDirectory = Path.Combine(Path.GetDirectoryName(processingItem.FileName), "TEMP");
                    string targetFileName = exporter.GenerateFileName(processingItem.FileName, targetDirectory);
                    exporter.Export(processingItem.FileName, targetFileName, processingItem.SliceStart, processingItem.SliceEnd);
                }
            //});

        }

        private readonly object queueLock = new object();

        private bool TryGetNextItem(out ProcessingItem processingItem)
        {
            lock (this.queueLock)
            {
                if (Queue.Count > 0)
                {
                    processingItem = Queue[0];
                    Queue.RemoveAt(0);
                    return true;
                }
                else
                {
                    processingItem = null;
                    return false;
                }
            }
        }
    }
}
