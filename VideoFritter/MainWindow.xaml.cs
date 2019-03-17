using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using VideoFritter.Controls.VideoPlayer;
using VideoFritter.ExportQueue;
using VideoFritter.Settings;

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
            this.viewModel.SliceEnd = this.videoPlayer.VideoLength;
        }

        private MainWindowViewModel viewModel;
        private ExportQueueWindow exportQueueWindow;
        private ExportQueueViewModel processingQueueViewModel = new ExportQueueViewModel();
        private SettingsDialog settingsDialog;


        private void Menu_File_Open(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void Menu_File_Export(object sender, RoutedEventArgs e)
        {
            this.viewModel.ExportCurrentSelection();
        }

        private void Menu_File_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Menu_View_Resize(object sender, RoutedEventArgs e)
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
            double horizontalResizeFactor = (maxWindowWidth - takenWidth) / this.videoPlayer.VideoWidth;
            double verticalResizeFactor = (maxWindowHeight - takenHeight) / this.videoPlayer.VideoHeight;
            double choosenResizeFactor = Math.Min(horizontalResizeFactor, verticalResizeFactor);

            // Set the size of the window
            this.Width = this.videoPlayer.VideoWidth * choosenResizeFactor + takenWidth;
            this.Height = this.videoPlayer.VideoHeight * choosenResizeFactor + takenHeight;

            // Move the window inside the screen if it went out
            this.Left = (currentScreen.Bounds.Width - this.Width) / 2;
            this.Top = (currentScreen.Bounds.Height - this.Height) / 2;
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

        private void VideoPlayer_VideoOpened(object sender, RoutedEventArgs e)
        {
            this.slider.Value = 0;
            this.viewModel.SliceStart = TimeSpan.Zero;
            this.viewModel.SliceEnd = this.videoPlayer.VideoLength;
        }

        private void OpenFile()
        {
            this.viewModel.OpenFile();
            if (this.viewModel.IsFileOpened)
            {
                this.videoPlayer.OpenFile(this.viewModel.OpenedFileName);
            }
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
            if (this.viewModel.SliceEnd < this.videoPlayer.VideoPosition)
            {
                this.viewModel.SliceEnd = this.videoPlayer.VideoLength;
            }

            this.viewModel.SliceStart = this.videoPlayer.VideoPosition;
        }

        private void SectionEndButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.videoPlayer.VideoPosition < this.viewModel.SliceStart)
            {
                this.viewModel.SliceStart = TimeSpan.Zero;
            }

            this.viewModel.SliceEnd = this.videoPlayer.VideoPosition;
        }

        private void AddToQueueButton_Click(object sender, RoutedEventArgs e)
        {
            processingQueueViewModel.AddToQueue(this.viewModel.OpenedFileName, this.viewModel.SliceStart, this.viewModel.SliceEnd);
        }

        private void SelectionPreviewButtonButton_Click(object sender, RoutedEventArgs e)
        {
            this.videoPlayer.Play(this.viewModel.SliceStart, this.viewModel.SliceEnd);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.videoPlayer.VideoPosition <= this.viewModel.SliceStart)
            {
                this.videoPlayer.VideoPosition = TimeSpan.Zero;
            }
            else if (this.videoPlayer.VideoPosition <= this.viewModel.SliceEnd)
            {
                this.videoPlayer.VideoPosition = this.viewModel.SliceStart;
            }
            else
            {
                this.videoPlayer.VideoPosition = this.viewModel.SliceEnd;
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.videoPlayer.VideoPosition >= this.viewModel.SliceEnd)
            {
                this.videoPlayer.VideoPosition = this.videoPlayer.VideoLength;
            }
            else if (this.videoPlayer.VideoPosition >= this.viewModel.SliceStart)
            {
                this.videoPlayer.VideoPosition = this.viewModel.SliceEnd;
            }
            else
            {
                this.videoPlayer.VideoPosition = this.viewModel.SliceStart;
            }
        }

        private void Menu_ExportQueue(object sender, RoutedEventArgs e)
        {
            if (this.exportQueueWindow == null)
            {
                this.exportQueueWindow = new ExportQueueWindow
                {
                    DataContext = this.processingQueueViewModel,
                };
                this.exportQueueWindow.Closed += ExportQueueWindow_Closed;
            }

            this.exportQueueWindow.Show();
            this.exportQueueWindow.Activate();
        }

        private void ExportQueueWindow_Closed(object sender, EventArgs e)
        {
            this.exportQueueWindow.Closed -= ExportQueueWindow_Closed;
            this.exportQueueWindow = null;
        }

        private void Menu_Settings(object sender, RoutedEventArgs e)
        {
            if (this.settingsDialog == null)
            {
                this.settingsDialog = new SettingsDialog();
                this.settingsDialog.Closed += SettingsDialog_Closed;
            }

            this.settingsDialog.Show();
            this.settingsDialog.Activate();
        }

        private void SettingsDialog_Closed(object sender, EventArgs e)
        {
            this.settingsDialog.Closed -= SettingsDialog_Closed;
            this.settingsDialog = null;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string fileName = files.FirstOrDefault();
                if (fileName != null)
                {
                    this.viewModel.OpenFile(fileName);
                    this.videoPlayer.OpenFile(fileName);
                }
            }
        }
    }
}
