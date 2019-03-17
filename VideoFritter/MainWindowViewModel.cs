using System;
using System.IO;
using System.Linq;
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
                if (value < SliceEnd)
                {
                    this.sliceStart = value;
                }
                else
                {
                    this.sliceStart = SliceEnd;
                }

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
                if (value > SliceStart)
                {
                    this.sliceEnd = value;
                }
                else
                {
                    this.sliceEnd = SliceStart;
                }

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

        public void OpenFile(string fileName)
        {
            OpenedFileName = fileName;
        }

        public void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Video files |*.mp4;*.mpg;*.mpeg;*.mov|All files |*.*"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                OpenFile(openFileDialog.FileName);
            }
        }

        public void ExportCurrentSelection()
        {
            if (string.IsNullOrEmpty(OpenedFileName))
            {
                return;
            }

            FFMpegExporter exporter = new FFMpegExporter();
            string targetFileName = exporter.GenerateFileName(OpenedFileName, ExportFilePath);

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = $"Video files |{string.Join("; ", SupportedFileExtensions.Select(ext => $"*.{ ext}"))}|All files |*.*",
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

                    if (task.IsFaulted)
                    {
                        App.DisplayUnexpectedError(task.Exception);
                        MessageBox.Show(task.Exception.ToString(), (string)System.Windows.Application.Current.Resources["ErrorDialogTitle"]);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());

                ExportFilePath = Path.GetDirectoryName(saveFileDialog.FileName);
            }
        }

        private static readonly string[] SupportedFileExtensions = { "mp4", "avi", "mov", "mpg", "mpeg", "mkv" };

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
