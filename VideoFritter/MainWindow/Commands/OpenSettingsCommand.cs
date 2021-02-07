using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

using VideoFritter.Settings;

namespace VideoFritter.MainWindow.Commands
{
    internal class OpenSettingsCommand : AbstractOpenDialogCommand
    {
        public OpenSettingsCommand(MainWindowViewModel mainWindowViewModelIn)
            : base(mainWindowViewModelIn)
        {
        }

        protected override Window CreateDialog()
        {
            return new SettingsDialog();
        }
    }
}
