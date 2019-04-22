using System;
using System.Windows.Input;

namespace VideoFritter.Settings
{
    internal class SaveCommand : ICommand
    {
        public SaveCommand(SettingsViewModel settingsViewModelIn)
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
}
