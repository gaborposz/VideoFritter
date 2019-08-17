using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

using VideoFritter.Common;
using VideoFritter.Controls.VideoPlayer;
using VideoFritter.Exporter;
using VideoFritter.MainWindow.Commands;
using VideoFritter.Properties;

namespace VideoFritter.MainWindow
{
    internal class MainWindowViewModel : AbstractExportingViewModel
    {
        public MainWindowViewModel(VideoPlayer videoPlayerIn)
        {
            OpenFileCommand = new OpenFileCommand(this, videoPlayerIn);
            PlayOrPauseCommand = new PlayOrPauseCommand(this, videoPlayerIn);
            SetSectionStartCommand = new SetSectionStartCommand(this, videoPlayerIn);
            SetSectionEndCommand = new SetSectionEndCommand(this, videoPlayerIn);
            BackwardCommand = new BackwardCommand(this, videoPlayerIn);
            ForwardCommand = new ForwardCommand(this, videoPlayerIn);
            StepBackwardCommand = new StepBackwardCommand(this, videoPlayerIn);
            StepForwardCommand = new StepForwardCommand(this, videoPlayerIn);
            PlaySelectionCommand = new PlaySelectionCommand(this, videoPlayerIn);
            ExportSelectionCommand = new ExportSelectionCommand(this, videoPlayerIn);
            NextVideoCommand = new NextVideoCommand(this, videoPlayerIn);
        }

        public static readonly string[] SupportedFileExtensions = { "mp4", "avi", "mov", "mpg", "mpeg", "mkv" };

        public event EventHandler<bool> IsFileOpenedChanged;

        public ICommand OpenFileCommand { get; }
        public ICommand PlayOrPauseCommand { get; }
        public ICommand SetSectionStartCommand { get; }
        public ICommand SetSectionEndCommand { get; }
        public ICommand BackwardCommand { get; }
        public ICommand ForwardCommand { get; }
        public ICommand StepBackwardCommand { get; }
        public ICommand StepForwardCommand { get; }
        public ICommand PlaySelectionCommand { get; }
        public ICommand ExportSelectionCommand { get; }
        public ICommand NextVideoCommand { get; }

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
            set
            {
                OnPropertyChanged(nameof(IsFileOpened));
                IsFileOpenedChanged?.Invoke(this, value);
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
                OnPropertyChanged(nameof(WindowTitle));
                IsFileOpened = true;
            }
        }

        public string WindowTitle
        {
            get
            {
                if (IsFileOpened)
                {
                    return $"{Resources.WindowTitle} - {OpenedFileName}";
                }
                else
                {
                    return Resources.WindowTitle;
                }
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
                Filter = GetFileDialogFilter(),
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
                Filter = GetFileDialogFilter(),
                InitialDirectory = Path.GetDirectoryName(targetFileName),
                RestoreDirectory = true,
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
                        MessageBox.Show(task.Exception.ToString(), (string)System.Windows.Application.Current.Resources["ErrorDialogTitle"]);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());

                ExportFilePath = Path.GetDirectoryName(saveFileDialog.FileName);
            }
        }

        private string openedFileName;
        private TimeSpan sliceStart;
        private TimeSpan sliceEnd;
        private string exportFilePath;

        private string GetFileDialogFilter()
        {
            return $"Video files |{string.Join("; ", SupportedFileExtensions.Select(ext => $"*.{ ext}"))}|All files |*.*";
        }

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
