using System;
using System.Windows;
using System.Windows.Input;

using VideoFritter.ProcessingQueue;
using VideoFritter.VideoPlayer;

namespace VideoFritter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.viewModel = (MainWindowViewModel)this.DataContext;
            this.viewModel.SliceEnd = this.videoPlayer.Length;
        }



        private MainWindowViewModel viewModel;
        private ProcessingQueueWindow processingQueueWindow;
        private ProcessingQueueViewModel processingQueueViewModel = new ProcessingQueueViewModel();


        private void Menu_File_Open(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void VideoPlayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.videoPlayer.PlayOrPause();
            if (e.ClickCount == 2)
            {
                OpenFile();
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            this.videoPlayer.PlayOrPause();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            this.videoPlayer.Stop();
        }

        private void VideoPlayer_VideoOpened(object sender, RoutedEventArgs e)
        {
            VideoOpenedEventArgs videoOpenedArgs = (VideoOpenedEventArgs)e;
            this.slider.Value = 0;
            this.viewModel.SliceStart = TimeSpan.Zero;
            this.viewModel.SliceEnd = this.videoPlayer.Length;

            CalculateWindowSizeToVideoSize(videoOpenedArgs.VideoWidth, videoOpenedArgs.VideoHeight);
        }

        private void OpenFile()
        {
            this.viewModel.OpenFile();
            if (this.viewModel.IsFileOpened)
            {
                this.videoPlayer.OpenFile(this.viewModel.OpenedFileName);
            }
        }

        private void CalculateWindowSizeToVideoSize(double videoWidth, double videoHeight)
        {
            const double horizontalScreenPadding = 50 * 2;
            const double verticalScreenPadding = 50 * 2;

            // Get the size of the current screen
            System.Windows.Forms.Screen currentScreen = System.Windows.Forms.Screen.FromRectangle(
                new System.Drawing.Rectangle((int)this.Left, (int)this.Top, (int)this.Width, (int)this.Height));

            // Calculate the space occupied by other controls
            double takenWidth = this.Width - this.videoPlayer.ActualVideoWidth;
            double takenHeight = this.Height - this.videoPlayer.ActualVideoHeight;

            // Calculate the maximum desirable window size
            double maxWindowWidth = currentScreen.Bounds.Width - horizontalScreenPadding;
            double maxWindowHeight = currentScreen.Bounds.Height - verticalScreenPadding;

            // Calculate the resize factor
            double horizontalResizeFactor = (maxWindowWidth - takenWidth) / videoWidth;
            double verticalResizeFactor = (maxWindowHeight - takenHeight) / videoHeight;
            double choosenResizeFactor = Math.Min(horizontalResizeFactor, verticalResizeFactor);

            // Set the size of the window
            this.Width = videoWidth * choosenResizeFactor + takenWidth;
            this.Height = videoHeight * choosenResizeFactor + takenHeight;

            // Move the window inside the screen if it went out
            this.Left = (currentScreen.Bounds.Width - this.Width) / 2;
            this.Top = (currentScreen.Bounds.Height - this.Height) / 2;
        }

        private void VideoPlayer_IsPlayingChanged(object sender, RoutedEventArgs e)
        {
            IsPlayingChangedEventArgs isPlayingChangedEventArgs = (IsPlayingChangedEventArgs)e;

            if (isPlayingChangedEventArgs.NewIsPlaying)
            {
                this.playButton.Content = "Pause";
            }
            else
            {
                this.playButton.Content = "Play";
            }
        }

        private void SectionStartButton_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.SliceStart = this.videoPlayer.Position;
        }

        private void SectionEndButton_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.SliceEnd = this.videoPlayer.Position;
        }

        private void Menu_Export_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.ExportCurrentSlice();
        }

        private void Menu_Queue_View_Click(object sender, RoutedEventArgs e)
        {
            this.processingQueueWindow = new ProcessingQueueWindow
            {
                DataContext = this.processingQueueViewModel,
            };
            this.processingQueueWindow.Show();
        }

        private void AddToQueueButton_Click(object sender, RoutedEventArgs e)
        {
            processingQueueViewModel.AddToQueue(this.viewModel.OpenedFileName, this.viewModel.SliceStart, this.viewModel.SliceEnd);
        }
    }
}
