using System;

using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal class SetSectionEndCommand : AbstractOpenedFileEnabledCommand
    {
        public SetSectionEndCommand(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn)
            : base(mainWindowViewModelIn)
        {
            VideoPlayer = videoPlayerIn;
        }

        public override void Execute(object parameter)
        {
            if (VideoPlayer.VideoPosition < MainWindowViewModel.SliceStart)
            {
                MainWindowViewModel.SliceStart = TimeSpan.Zero;
            }

            MainWindowViewModel.SliceEnd = VideoPlayer.VideoPosition;
        }

        private VideoPlayer VideoPlayer { get; }
    }
}
