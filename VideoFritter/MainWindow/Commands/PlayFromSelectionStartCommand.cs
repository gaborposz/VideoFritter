using System;

using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal class PlayFromSelectionStartCommand : AbstractOpenedFileEnabledCommand
    {
        public PlayFromSelectionStartCommand(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn)
            : base(mainWindowViewModelIn)
        {
            VideoPlayer = videoPlayerIn;
        }

        public override void Execute(object parameter)
        {
            VideoPlayer.Play(MainWindowViewModel.SliceStart, TimeSpan.MaxValue);
        }

        private VideoPlayer VideoPlayer { get; }
    }
}
