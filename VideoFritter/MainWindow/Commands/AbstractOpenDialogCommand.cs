using System;
using System.Windows;

namespace VideoFritter.MainWindow.Commands
{
    internal abstract class AbstractOpenDialogCommand : AbstractMainWindowCommandBase
    {
        public AbstractOpenDialogCommand(MainWindowViewModel mainWindowViewModelIn)
            : base(mainWindowViewModelIn)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            if (this.dialog == null)
            {
                this.dialog = CreateDialog();
                this.dialog.Closed += Dialog_Closed;
            }

            this.dialog.Show();
            this.dialog.Activate();
        }

        protected abstract Window CreateDialog();

        private Window dialog;

        private void Dialog_Closed(object sender, EventArgs e)
        {
            this.dialog.Closed -= Dialog_Closed;
            this.dialog = null;
        }
    }
}
