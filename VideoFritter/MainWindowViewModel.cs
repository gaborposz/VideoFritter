using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using VideoFritter.Common;
using VideoFritter.Exporter;

namespace VideoFritter
{
    internal class MainWindowViewModel : AbstractExportingViewModel
    {
        public TimeSpan SliceStart
        {
            get
            {
                return this.sliceStart;
            }

            set
            {
                this.sliceStart = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan SliceEnd
        {
            get
            {
                return this.sliceEnd;
            }

            set
            {
                this.sliceEnd = value;
                OnPropertyChanged();
            }
        }

        public bool IsFileOpened
        {
            get
            {
                return OpenedFileName != null;
            }
        }

        public string OpenedFileName
        {
            get
            {
                return this.openedFileName;
            }
            private set
            {
                this.openedFileName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsFileOpened));
            }
        }

        public void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Video files |*.mp4;*.mpg;*.mpeg;*.mov|All files |*.*"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                OpenedFileName = openFileDialog.FileName;
            }
        }

        public void ExportCurrentSlice()
        {
            if (string.IsNullOrEmpty(OpenedFileName))
            {
                throw new InvalidOperationException("Export is not available if nothing is opened.");
            }

            FFMpegExporter exporter = new FFMpegExporter();
            string targetFileName = exporter.GenerateFileName(OpenedFileName, ExportFilePath);

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Video files |*.mp4;*.mpg;*.mpeg;*.mov|All files |*.*",
                FileName = targetFileName,
                DefaultExt = Path.GetExtension(targetFileName),
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                IsExporting = true;
                Task exportTask = exporter.ExportAsync(OpenedFileName, saveFileDialog.FileName, SliceStart, SliceEnd, CancellationToken.None, this);
                exportTask.ContinueWith((task) =>
                {
                    IsExporting = false;
                    ExportProgress = 0; // Reset the progress bar
                }, TaskScheduler.FromCurrentSynchronizationContext());

                ExportFilePath = Path.GetDirectoryName(saveFileDialog.FileName);
            }
        }

        private string openedFileName;
        private TimeSpan sliceStart;
        private TimeSpan sliceEnd;
        private string exportFilePath;

        private string ExportFilePath
        {
            get
            {
                if (this.exportFilePath == null || !Directory.Exists(this.exportFilePath))
                {
                    this.exportFilePath = Path.GetDirectoryName(OpenedFileName);
                }

                return this.exportFilePath;
            }

            set
            {
                this.exportFilePath = value;
            }
        }

    }
}
