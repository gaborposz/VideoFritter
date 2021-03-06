﻿using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace VideoFritter.About
{
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : Window
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void OpenWebURL(object sender, RequestNavigateEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd";
            startInfo.Arguments = $"/c start {e.Uri.AbsoluteUri.Replace("&", "^&")}";
            startInfo.CreateNoWindow = true;

            Process.Start(startInfo);

            e.Handled = true;
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            Image image = (Image)sender;
            MemoryStream memoryStream = new MemoryStream();
            Properties.Resources.VideoFritterIcon.Save(memoryStream);
            image.Source = BitmapFrame.Create(memoryStream);

        }

        private void WindowDragging(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
