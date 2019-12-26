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
            return this.settingsViewModel.ExportQueuePath != ApplicationSettings.ExportQueuePath ||
                this.settingsViewModel.TimeStampCorrection != ApplicationSettings.TimeStampCorrection ||
                this.settingsViewModel.SaveFFMpegLogs != ApplicationSettings.SaveFFMpegLogs;
        }

        public void Execute(object parameter)
        {
           ApplicationSettings.ExportQueuePath = this.settingsViewModel.ExportQueuePath;
           ApplicationSettings.TimeStampCorrection = this.settingsViewModel.TimeStampCorrection;
           ApplicationSettings.SaveFFMpegLogs = this.settingsViewModel.SaveFFMpegLogs;
           ApplicationSettings.Save();

            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private readonly SettingsViewModel settingsViewModel;
    }
}
