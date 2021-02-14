using System;
using System.Windows;

using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal class SeekCommand : AbstractOpenedFileEnabledCommand
    {
        public SeekCommand(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn, Func<TimeSpan> getSeekTimeIn)
            : base(mainWindowViewModelIn)
        {
            this.videoPlayer = videoPlayerIn;

            this.getSeekTime = getSeekTimeIn;

            this.videoPlayer.IsPlayingChanged += VideoPlayer_IsPlayingChanged;
            this.videoPlayer.VideoPositionChanged += VideoPlayer_VideoPositionChanged;
        }

        public override bool CanExecute(object parameter)
        {
            return base.CanExecute(parameter) &&
                !this.videoPlayer.IsPlaying &&
                this.videoPlayer.VideoPosition > TimeSpan.Zero &&
                this.videoPlayer.VideoPosition < this.videoPlayer.VideoLength;
        }

        public override void Execute(object parameter)
        {
            TimeSpan timeToSeek = getSeekTime();
            TimeSpan desiredPosition = this.videoPlayer.VideoPosition.Add(timeToSeek);

            if (timeToSeek.Ticks > 0)
            {
                if (desiredPosition > this.videoPlayer.VideoLength)
                {
                    desiredPosition = this.videoPlayer.VideoLength;
                }
            }
            else
            {
                if (desiredPosition < TimeSpan.Zero)
                {
                    desiredPosition = TimeSpan.Zero;
                }
            }

            this.videoPlayer.VideoPosition = desiredPosition;
        }

        private VideoPlayer videoPlayer;
        private Func<TimeSpan> getSeekTime;

        private void VideoPlayer_VideoPositionChanged(object sender, RoutedEventArgs e)
        {
            SendCanExecuteChanged();
        }

        private void VideoPlayer_IsPlayingChanged(object sender, RoutedEventArgs e)
        {
            SendCanExecuteChanged();
        }
    }
}
