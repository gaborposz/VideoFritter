using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using VideoFritter.About;
using VideoFritter.Controls.VideoPlayer;
using VideoFritter.ExportQueue;
using VideoFritter.Settings;

namespace VideoFritter.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.viewModel = new MainWindowViewModel(this.videoPlayer);
            this.DataContext = this.viewModel;

            this.viewModel.SliceEnd = this.videoPlayer.VideoLength;
        }

        private MainWindowViewModel viewModel;

        private void Menu_File_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void VideoPlayer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.viewModel.PlayOrPauseCommand.Execute(null);

            if (e.ClickCount == 2)
            {
                this.viewModel.OpenFileCommand.Execute(null);
            }
        }

        private void VideoPlayer_VideoOpened(object sender, RoutedEventArgs e)
        {
            this.slider.Value = 0;
            this.viewModel.SliceStart = TimeSpan.Zero;
            this.viewModel.SliceEnd = this.videoPlayer.VideoLength;
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

        private void MaximizeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void MinimizeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            this.viewModel.SaveSettings();
        }
    }
}
