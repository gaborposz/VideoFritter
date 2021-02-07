
using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal class SetSectionStartCommand : AbstractOpenedFileEnabledCommand
    {
        public SetSectionStartCommand(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn)
            : base(mainWindowViewModelIn)
        {
            VideoPlayer = videoPlayerIn;
        }

        public override void Execute(object parameter)
        {
            if (MainWindowViewModel.SliceEnd < VideoPlayer.VideoPosition)
            {
                MainWindowViewModel.SliceEnd = VideoPlayer.VideoLength;
            }

            MainWindowViewModel.SliceStart = VideoPlayer.VideoPosition;
        }

        private VideoPlayer VideoPlayer { get; }
    }
}
