using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace VideoFritter.VideoPlayer
{
    /// <summary>
    /// Interaction logic for VideoPlayer.xaml
    /// </summary>
    public partial class VideoPlayer : UserControl
    {
        public VideoPlayer()
        {
            InitializeComponent();

            this.videoPlaybackTimer = new DispatcherTimer();
            this.videoPlaybackTimer.Tick += VideoPlaybackTimer_Tick;
            this.videoPlaybackTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
        }

        public static readonly RoutedEvent VideoOpenedEvent =
            EventManager.RegisterRoutedEvent("VideoOpened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VideoPlayer));

        public static readonly RoutedEvent IsPlayingChangedEvent =
            EventManager.RegisterRoutedEvent("IsPlayingChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VideoPlayer));

        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register(
                "VideoLength",
                typeof(TimeSpan),
                typeof(VideoPlayer),
                new PropertyMetadata(TimeSpan.FromSeconds(10)));

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(
                "VideoPosition",
                typeof(TimeSpan),
                typeof(VideoPlayer),
                new FrameworkPropertyMetadata(TimeSpan.Zero, PositionPropertyChangedCallback));

        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register(
                "VideoVolume",
                typeof(double),
                typeof(VideoPlayer),
                new FrameworkPropertyMetadata(0d, VolumePropertyChangedCallback));

        public TimeSpan VideoLength
        {
            get
            {
                return (TimeSpan)GetValue(LengthProperty);
            }
        }

        public TimeSpan VideoPosition
        {
            get
            {
                return (TimeSpan)GetValue(PositionProperty);
            }
            set
            {
                SetValue(PositionProperty, value);
            }
        }

        public double VideoVolume
        {
            get
            {
                return (double)GetValue(VolumeProperty);
            }
            set
            {
                SetValue(VolumeProperty, value);
            }
        }

        public event RoutedEventHandler VideoOpened
        {
            add { AddHandler(VideoOpenedEvent, value); }
            remove { RemoveHandler(VideoOpenedEvent, value); }
        }

        public event RoutedEventHandler IsPlayingChanged
        {
            add { AddHandler(IsPlayingChangedEvent, value); }
            remove { RemoveHandler(IsPlayingChangedEvent, value); }
        }

        public int VideoWidth
        {
            get
            {
                return this.mediaElement.NaturalVideoWidth;
            }
        }

        public int VideoHeight
        {
            get
            {
                return this.mediaElement.NaturalVideoHeight;
            }
        }

        public double ActualVideoWidth
        {
            get
            {
                return this.videoBorder.ActualWidth - this.videoBorder.BorderThickness.Left - this.videoBorder.BorderThickness.Right;
            }
        }

        public double ActualVideoHeight
        {
            get
            {
                return this.videoBorder.ActualHeight - this.videoBorder.BorderThickness.Top - this.videoBorder.BorderThickness.Bottom;
            }
        }

        public bool IsEnded
        {
            get
            {
                return this.mediaElement.NaturalDuration.TimeSpan == this.mediaElement.Position;
            }
        }

        public void OpenFile(string fileName)
        {
            this.mediaElement.Source = new Uri(fileName);
            this.mediaElement.Play();
            this.mediaElement.Pause();
        }

        public void PlayOrPause()
        {
            if (!this.isVideoLoaded)
            {
                return;
            }

            if (IsPlaying)
            {
                this.mediaElement.Pause();
                Thread.Sleep(50);
                this.desiredPosition = this.mediaElement.Position;
                IsPlaying = false;
            }
            else
            {
                if (this.mediaElement.Position != this.desiredPosition)
                {
                    this.mediaElement.Position = this.desiredPosition;
                }

                this.mediaElement.Play();
                IsPlaying = true;
            }
        }

        public void Stop()
        {
            this.mediaElement.Stop();
            IsPlaying = false;
            SetValue(PositionProperty, TimeSpan.Zero);
        }

        protected virtual void RaiseVideoOpenedEvent()
        {
            VideoOpenedEventArgs args = new VideoOpenedEventArgs(
                VideoOpenedEvent,
                this,
                this.mediaElement.NaturalVideoWidth,
                this.mediaElement.NaturalVideoHeight);

            RaiseEvent(args);
        }

        protected virtual void RaiseIsPlayingChangedEvent()
        {
            IsPlayingChangedEventArgs args = new IsPlayingChangedEventArgs(
                IsPlayingChangedEvent,
                this,
                this.isPlaying);

            RaiseEvent(args);
        }

        private DispatcherTimer videoPlaybackTimer;
        private bool isPlaying;
        private TimeSpan desiredPosition;
        private bool isVideoLoaded = false;
        private bool positionSetByTimer;

        private static void PositionPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VideoPlayer videoPlayer = (VideoPlayer)d;
            TimeSpan newValue = (TimeSpan)e.NewValue;

            if (!videoPlayer.isVideoLoaded || videoPlayer.positionSetByTimer)
            {
                return;
            }

            videoPlayer.SetValue(PositionProperty, newValue);

            videoPlayer.desiredPosition = newValue;
            videoPlayer.mediaElement.Position = newValue;
        }

        private static void VolumePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VideoPlayer videoPlayer = (VideoPlayer)d;
            double newValue = (double)e.NewValue;

            videoPlayer.mediaElement.Volume = newValue;
            videoPlayer.SetValue(VolumeProperty, newValue);
        }

        private bool IsPlaying
        {
            get
            {
                return this.isPlaying;
            }
            set
            {
                this.isPlaying = value;
                RaiseIsPlayingChangedEvent();

                if (this.isPlaying)
                {
                    this.videoPlaybackTimer.Start();
                }
                else
                {
                    this.videoPlaybackTimer.Stop();
                }
            }
        }

        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            this.isVideoLoaded = true;
            IsPlaying = false;
            SetValue(LengthProperty, this.mediaElement.NaturalDuration.TimeSpan);
            SetValue(VolumeProperty, this.mediaElement.Volume);

            RaiseVideoOpenedEvent();
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.mediaElement.Pause();
            this.desiredPosition = TimeSpan.Zero;
            IsPlaying = false;
        }

        private void VideoPlaybackTimer_Tick(object sender, EventArgs e)
        {
            this.positionSetByTimer = true;
            SetValue(PositionProperty, this.mediaElement.Position);
            this.positionSetByTimer = false;
        }
    }
}
