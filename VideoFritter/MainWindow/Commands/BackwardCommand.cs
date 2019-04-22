using System;

using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal class BackwardCommand : AbstractOpenedFileEnabledCommand
    {
        public BackwardCommand(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn)
            : base(mainWindowViewModelIn, videoPlayerIn)
        {
        }

        public override void Execute(object parameter)
        {
            if (VideoPlayer.VideoPosition <= MainWindowViewModel.SliceStart)
            {
                VideoPlayer.VideoPosition = TimeSpan.Zero;
            }
            else if (VideoPlayer.VideoPosition <= MainWindowViewModel.SliceEnd)
            {
                VideoPlayer.VideoPosition = MainWindowViewModel.SliceStart;
            }
            else
            {
                VideoPlayer.VideoPosition = MainWindowViewModel.SliceEnd;
            }
        }
    }
}
