using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VideoFritter.Controls.AudioVolumeControl
{
    public class AudioVolumeControl : ProgressBar
    {
        static AudioVolumeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AudioVolumeControl), new FrameworkPropertyMetadata(typeof(AudioVolumeControl)));
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.MouseLeftButtonDown += AudioVolumeControl_MouseLeftButtonDown;
            this.MouseMove += AudioVolumeControl_MouseMove;
        }

        private void AudioVolumeControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                UpdateProgressBarValue((ProgressBar)sender, e);
            }
        }

        private void AudioVolumeControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UpdateProgressBarValue((ProgressBar)sender, e);
        }

        private void UpdateProgressBarValue(ProgressBar progressBar, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(progressBar);
            progressBar.Value = progressBar.Maximum * (progressBar.ActualHeight - mousePosition.Y) / progressBar.ActualHeight;
        }
    }
}
