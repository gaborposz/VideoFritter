using System;
using System.Windows.Input;

using VideoFritter.Common;

namespace VideoFritter.Settings
{
    internal class SettingsViewModel : AbstractViewModelBase
    {
        public SettingsViewModel()
        {
            SaveCommand = new SaveCommandImp(this);
            ResetToDefaultsCommand = new ResetToDefaultsCommandImp(this);

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

        private class SaveCommandImp : ICommand
        {
            public SaveCommandImp(SettingsViewModel settingsViewModelIn)
            {
                this.settingsViewModel = settingsViewModelIn;
                this.settingsViewModel.PropertyChanged += SettingsViewModel_PropertyChanged;

            }

            private void SettingsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return this.settingsViewModel.ExportQueuePath != Properties.Settings.Default.ExportQueuePath ||
                    this.settingsViewModel.TimeStampCorrection != Properties.Settings.Default.TimeStampCorrection ||
                    this.settingsViewModel.SaveFFMpegLogs != Properties.Settings.Default.SaveFFMpegLogs;
            }

            public void Execute(object parameter)
            {
                Properties.Settings.Default.ExportQueuePath = this.settingsViewModel.ExportQueuePath;
                Properties.Settings.Default.TimeStampCorrection = this.settingsViewModel.TimeStampCorrection;
                Properties.Settings.Default.SaveFFMpegLogs = this.settingsViewModel.SaveFFMpegLogs;
                Properties.Settings.Default.Save();

                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }

            private readonly SettingsViewModel settingsViewModel;
        }

        private class ResetToDefaultsCommandImp : ICommand
        {
            public ResetToDefaultsCommandImp(SettingsViewModel settingsViewModelIn)
            {
                this.settingsViewModel = settingsViewModelIn;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                this.settingsViewModel.ExportQueuePath = @"$(VideoPath)\Export";
                this.settingsViewModel.TimeStampCorrection = true;
                this.settingsViewModel.SaveFFMpegLogs = false;
            }

            private readonly SettingsViewModel settingsViewModel;
        }
    }
}
