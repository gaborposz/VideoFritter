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
using VideoFritter.ExportQueue;
using VideoFritter.MainWindow.Commands;
using VideoFritter.Properties;

namespace VideoFritter.MainWindow
{
    internal class MainWindowViewModel : AbstractExportingViewModel
    {
        public MainWindowViewModel(VideoPlayer videoPlayerIn)
        {
            this.exportQueueViewModel = new ExportQueueViewModel();

            OpenFileCommand = new OpenFileCommand(this, videoPlayerIn);
            OpenExportQueueCommand = new OpenExportQueueCommand(this, this.exportQueueViewModel);
            OpenSettingsCommand = new OpenSettingsCommand(this);
            OpenAboutCommand = new OpenAboutCommand(this);
            PlayOrPauseCommand = new PlayOrPauseCommand(this, videoPlayerIn);
            SetSectionStartCommand = new SetSectionStartCommand(this, videoPlayerIn);
            SetSectionEndCommand = new SetSectionEndCommand(this, videoPlayerIn);
            BackwardCommand = new BackwardCommand(this, videoPlayerIn);
            ForwardCommand = new ForwardCommand(this, videoPlayerIn);
            StepBackwardCommand = new SeekCommand(this, videoPlayerIn, () => -videoPlayerIn.FrameTime);
            StepForwardCommand = new SeekCommand(this, videoPlayerIn, () => videoPlayerIn.FrameTime);
            Step1sBackwardCommand = new SeekCommand(this, videoPlayerIn, () => -TimeSpan.FromSeconds(1));
            Step1sForwardCommand = new SeekCommand(this, videoPlayerIn, () => TimeSpan.FromSeconds(1));
            PlayFromSelectionStartCommand = new PlayFromSelectionStartCommand(this, videoPlayerIn);
            PlaySelectionCommand = new PlaySelectionCommand(this, videoPlayerIn);
            PlayUntilSelectionEndCommand = new PlayUntilSelectionEndCommand(this, videoPlayerIn);
            ExportSelectionCommand = new ExportSelectionCommand(this);
            NextVideoCommand = new NextVideoCommand(this, videoPlayerIn);
            AddToQueueCommand = new AddToQueueCommand(this, this.exportQueueViewModel);

            AudioVolume = ApplicationSettings.AudioVolume;
        }

        public static readonly string[] SupportedFileExtensions = { "mp4", "avi", "mov", "mpg", "mpeg", "mkv" };

        public event EventHandler<bool> IsFileOpenedChanged;

        public ICommand OpenFileCommand { get; }
        public ICommand OpenExportQueueCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand OpenAboutCommand { get; }
        public ICommand PlayOrPauseCommand { get; }
        public ICommand SetSectionStartCommand { get; }
        public ICommand SetSectionEndCommand { get; }
        public ICommand BackwardCommand { get; }
        public ICommand ForwardCommand { get; }
        public ICommand StepBackwardCommand { get; }
        public ICommand StepForwardCommand { get; }
        public ICommand Step1sBackwardCommand { get; }
        public ICommand Step1sForwardCommand { get; }
        public ICommand PlayFromSelectionStartCommand { get; }
        public ICommand PlaySelectionCommand { get; }
        public ICommand PlayUntilSelectionEndCommand { get; }
        public ICommand ExportSelectionCommand { get; }
        public ICommand NextVideoCommand { get; }
        public ICommand AddToQueueCommand { get; }

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
                OnPropertyChanged(nameof(SliceLength));
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
                OnPropertyChanged(nameof(SliceLength));
            }
        }

        public TimeSpan SliceLength
        {
            get
            {
                return SliceEnd - SliceStart;
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
                    return $"{Resources.WindowTitle} - {EscapeUnderscores(OpenedFileName)}";
                }
                else
                {
                    return Resources.WindowTitle;
                }
            }
        }

        public double AudioVolume
        {
            get
            {
                return this.audioVolume;
            }

            set
            {
                this.audioVolume = value;
                OnPropertyChanged();
            }
        }

        public void OpenFile(string fileName)
        {
            OpenedFileName = fileName;
        }

        public bool OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = GetFileDialogFilter(),
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                OpenFile(openFileDialog.FileName);
                return true;
            }

            return false;
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

        public void SaveSettings()
        {
            if (AudioVolume != ApplicationSettings.AudioVolume)
            {
                ApplicationSettings.AudioVolume = AudioVolume;
                ApplicationSettings.Save();
            }
        }

        private readonly ExportQueueViewModel exportQueueViewModel;

        private string openedFileName;
        private TimeSpan sliceStart;
        private TimeSpan sliceEnd;
        private string exportFilePath;
        private double audioVolume;

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

        private static string EscapeUnderscores(string inputText)
        {
            // Workaround, because in the Window title the underscore characters 
            // are also interpreted as "hotkeys", just like in menu texts.
            return inputText.Replace("_", "__");
        }
    }
}
