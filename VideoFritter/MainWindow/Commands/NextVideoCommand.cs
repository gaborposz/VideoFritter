using System.Collections.Generic;
using System.IO;

using VideoFritter.Controls.VideoPlayer;

namespace VideoFritter.MainWindow.Commands
{
    internal class NextVideoCommand : AbstractOpenedFileEnabledCommand
    {
        public NextVideoCommand(MainWindowViewModel mainWindowViewModelIn, VideoPlayer videoPlayerIn)
            : base(mainWindowViewModelIn)
        {
            VideoPlayer = videoPlayerIn;
            MainWindowViewModel.IsFileOpenedChanged += MainWindowViewModel_IsFileOpenedChanged;
        }

        public override bool CanExecute(object parameter)
        {
            return base.CanExecute(parameter) && this.isThereOneMoreFile;
        }

        public override void Execute(object parameter)
        {
            int currentVideoIndex = UpdateVideoList();

            int nextVideoIndex = currentVideoIndex + 1;
            if (nextVideoIndex < this.videosInCurrentFolder.Count)
            {
                MainWindowViewModel.OpenFile(this.videosInCurrentFolder[nextVideoIndex]);
                VideoPlayer.OpenFile(MainWindowViewModel.OpenedFileName);
            }

            this.isThereOneMoreFile = nextVideoIndex + 1 < this.videosInCurrentFolder.Count;
            SendCanExecuteChanged();
        }

        private VideoPlayer VideoPlayer { get; }
        private bool isThereOneMoreFile = true;
        private readonly IList<string> videosInCurrentFolder = new List<string>();

        private int UpdateVideoList()
        {
            int currentVideoIndex = -1;
            string currentDirectory = Path.GetDirectoryName(MainWindowViewModel.OpenedFileName);
            this.videosInCurrentFolder.Clear();

            foreach (string file in Directory.EnumerateFiles(currentDirectory))
            {
                if (IsSupportedFile(file))
                {
                    this.videosInCurrentFolder.Add(file);

                    if (file == MainWindowViewModel.OpenedFileName)
                    {
                        currentVideoIndex = this.videosInCurrentFolder.Count - 1;
                    }
                }
            }

            return currentVideoIndex;
        }

        private void MainWindowViewModel_IsFileOpenedChanged(object sender, bool newValue)
        {
            if (newValue == true)
            {
                int currentVideoIndex = UpdateVideoList();

                this.isThereOneMoreFile = currentVideoIndex + 1 < this.videosInCurrentFolder.Count;
                SendCanExecuteChanged();
            }
        }

        private bool IsSupportedFile(string file)
        {
            string extension = Path.GetExtension(file).Remove(0, 1);

            foreach (string supportedExtension in MainWindowViewModel.SupportedFileExtensions)
            {
                if (string.Compare(extension, supportedExtension, true) == 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
