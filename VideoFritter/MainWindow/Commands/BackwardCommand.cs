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
            else if (VideoPlayer.VideoPosition <= MainWindowViewModel.SliceEnd
                // WORKAROUND: This second condition is needed because 
                // the VideoPosition can be actually beyond the length of the video,
                // and it that case it has to go to "SliceStart" instead of "SliceEnd".
                || MainWindowViewModel.SliceEnd == VideoPlayer.VideoLength)
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
