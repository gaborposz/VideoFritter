using System.Windows.Input;

using VideoFritter.Common;

namespace VideoFritter.Settings
{
    internal class SettingsViewModel : AbstractViewModelBase
    {
        public SettingsViewModel()
        {
            SaveCommand = new SaveCommand(this);
            ResetToDefaultsCommand = new ResetToDefaultsCommand(this);

            ExportQueuePath = Properties.Settings.Default.ExportQueuePath;
            TimeStampCorrection = Properties.Settings.Default.TimeStampCorrection;
            SaveFFMpegLogs = Properties.Settings.Default.SaveFFMpegLogs;
        }

        public ICommand SaveCommand { get; }

        public ICommand ResetToDefaultsCommand { get; }

        public string ExportQueuePath
        {
            get
            {
                return this.exportQueuePath;
            }

            set
            {
                this.exportQueuePath = value;
                OnPropertyChanged();
            }
        }

        public bool TimeStampCorrection
        {
            get
            {
                return this.timeStampCorrection;
            }

            set
            {
                this.timeStampCorrection = value;
                OnPropertyChanged();
            }
        }

        public bool SaveFFMpegLogs
        {
            get
            {
                return this.saveFFMpegLogs;
            }

            set
            {
                this.saveFFMpegLogs = value;
                OnPropertyChanged();
            }
        }

        private string exportQueuePath;
        private bool timeStampCorrection;
        private bool saveFFMpegLogs;
    }
}
