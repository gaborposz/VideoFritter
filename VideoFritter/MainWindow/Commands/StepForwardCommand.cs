using System;

using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal class StepForwardCommand : AbstractSteppingCommand
    {
        public StepForwardCommand(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn)
            : base(mainWindowViewModelIn, videoPlayerIn)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return base.CanExecute(parameter) &&
                !VideoPlayer.IsPlaying &&
                VideoPlayer.VideoPosition < VideoPlayer.VideoLength;
        }

        public override void Execute(object parameter)
        {
            TimeSpan desiredPosition = VideoPlayer.VideoPosition.Add(FrameSteppingInterval);
            if (desiredPosition > VideoPlayer.VideoLength)
            {
                desiredPosition = VideoPlayer.VideoLength;
            }

            VideoPlayer.VideoPosition = desiredPosition;
        }
    }
}
