using System.Windows;

using VideoFritter.About;

namespace VideoFritter.MainWindow.Commands
{
    internal class OpenAboutCommand : AbstractOpenDialogCommand
    {
        public OpenAboutCommand(MainWindowViewModel mainWindowViewModelIn)
            : base(mainWindowViewModelIn)
        {
        }


        protected override Window CreateDialog()
        {
            return new AboutDialog();
        }
    }
}
