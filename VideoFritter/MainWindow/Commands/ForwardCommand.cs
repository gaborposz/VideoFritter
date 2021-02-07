using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal class ForwardCommand : AbstractOpenedFileEnabledCommand
    {
        public ForwardCommand(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn)
            : base(mainWindowViewModelIn)
        {
            VideoPlayer = videoPlayerIn;
        }

        public override void Execute(object parameter)
        {
            if (VideoPlayer.VideoPosition >= MainWindowViewModel.SliceEnd)
            {
                VideoPlayer.VideoPosition = VideoPlayer.VideoLength;
            }
            else if (VideoPlayer.VideoPosition >= MainWindowViewModel.SliceStart)
            {
                VideoPlayer.VideoPosition = MainWindowViewModel.SliceEnd;
            }
            else
            {
                VideoPlayer.VideoPosition = MainWindowViewModel.SliceStart;
            }
        }

        private VideoPlayer VideoPlayer { get; }
    }
}
