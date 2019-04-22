using System;

using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal class StepBackwardCommand : AbstractSteppingCommand
    {
        public StepBackwardCommand(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn)
            : base(mainWindowViewModelIn, videoPlayerIn)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return base.CanExecute(parameter) &&
                !VideoPlayer.IsPlaying &&
                VideoPlayer.VideoPosition > TimeSpan.Zero;
        }

        public override void Execute(object parameter)
        {
            TimeSpan desiredPosition = VideoPlayer.VideoPosition.Subtract(FrameSteppingInterval);
            if (desiredPosition < TimeSpan.Zero)
            {
                desiredPosition = TimeSpan.Zero;
            }

            VideoPlayer.VideoPosition = desiredPosition;
        }
    }
}
