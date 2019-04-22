using System;

using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal class SetSectionEndCommand : AbstractOpenedFileEnabledCommand
    {
        public SetSectionEndCommand(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn)
            : base(mainWindowViewModelIn, videoPlayerIn)
        {
        }

        public override void Execute(object parameter)
        {
            if (VideoPlayer.VideoPosition < MainWindowViewModel.SliceStart)
            {
                MainWindowViewModel.SliceStart = TimeSpan.Zero;
            }

            MainWindowViewModel.SliceEnd = VideoPlayer.VideoPosition;
        }
    }
}
