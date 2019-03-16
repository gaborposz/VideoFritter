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

        public bool HasItems
        {
            get
            {
                return Queue.Count > 0;
            }
        }

        public bool HasSelection
        {
            get
            {
                return SelectedIndex != -1;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return this.selectedIndex;
            }
            set
            {
                this.selectedIndex = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelection));
            }
        }

        public void ClearQueue()
        {
            Queue.Clear();
            OnPropertyChanged(nameof(HasItems));
        }

        public void RemoveSelectedItem()
        {
            Queue.RemoveAt(SelectedIndex);
            OnPropertyChanged(nameof(HasItems));
        }

        public void AddToQueue(string fileName, TimeSpan sliceStart, TimeSpan sliceEnd)
        {
            Queue.Add(new ExportItem(fileName, sliceStart, sliceEnd));
            OnPropertyChanged(nameof(HasItems));
        }

        public void ExportQueue()
        {
            if (Queue.Count > 0)
            {
                IsExporting = true;
                ExportItem processingItem = Queue[0];

                string sourceDirectory = Path.GetDirectoryName(processingItem.FileName);
                string exportPath = Properties.Settings.Default.ExportQueuePath.Replace("$(VideoPath)", sourceDirectory);
                string targetFileName = this.exporter.GenerateFileName(processingItem.FileName, exportPath);
                Task exportTask = this.exporter.ExportAsync(processingItem.FileName, targetFileName, processingItem.SliceStart, processingItem.SliceEnd, CancellationToken.None, this);
                exportTask.ContinueWith((task) =>
                {
                    Queue.Remove(processingItem);
                    OnPropertyChanged(nameof(HasItems));
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

        private int selectedIndex;
    }
}
