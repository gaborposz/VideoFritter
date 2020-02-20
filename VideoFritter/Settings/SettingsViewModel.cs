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

            ExportQueuePath = ApplicationSettings.ExportQueuePath;
            TimeStampCorrection = ApplicationSettings.TimeStampCorrection;
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

        private string exportQueuePath;
        private bool timeStampCorrection;
    }
}
