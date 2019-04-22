using System;
using System.Windows.Input;

using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal abstract class AbstractMainWindowCommandBase : ICommand
    {
        public AbstractMainWindowCommandBase(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn)
        {
            MainWindowViewModel = mainWindowViewModelIn;
            VideoPlayer = videoPlayerIn;
        }

        public event EventHandler CanExecuteChanged;

        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);

        protected MainWindowViewModel MainWindowViewModel { get; private set; }
        protected VideoPlayer VideoPlayer { get; private set; }

        protected void SendCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
