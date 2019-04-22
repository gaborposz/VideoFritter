using System;
using System.Windows.Input;

namespace VideoFritter.Settings
{
    internal class ResetToDefaultsCommand : ICommand
    {
        public ResetToDefaultsCommand(SettingsViewModel settingsViewModelIn)
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
