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
                UpdateSaveCommandCanExecute();
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
                UpdateSaveCommandCanExecute();
            }
        }

        public bool HasChanges()
        {
            return this.exportQueuePath != Properties.Settings.Default.ExportQueuePath ||
                this.timeStampCorrection != Properties.Settings.Default.TimeStampCorrection;
        }

        public void Save()
        {
            Properties.Settings.Default.ExportQueuePath = ExportQueuePath;
            Properties.Settings.Default.TimeStampCorrection = TimeStampCorrection;
            Properties.Settings.Default.Save();
            UpdateSaveCommandCanExecute();
        }

        private string exportQueuePath;
        private bool timeStampCorrection;

        private class SaveCommandImp : ICommand
        {
            public SaveCommandImp(SettingsViewModel settingsViewModelIn)
            {
                this.settingsViewModel = settingsViewModelIn;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return this.settingsViewModel.HasChanges();
            }

            public void Execute(object parameter)
            {
                this.settingsViewModel.Save();
            }

            public void UpdateCanExecute()
            {
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
            }

            private readonly SettingsViewModel settingsViewModel;
        }

        private void UpdateSaveCommandCanExecute()
        {
            ((SaveCommandImp)SaveCommand).UpdateCanExecute();
        }
    }
}
