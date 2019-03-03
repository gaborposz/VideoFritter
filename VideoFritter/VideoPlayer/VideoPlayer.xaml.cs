using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

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

            this.mediaTimeline = new MediaTimeline();
            this.mediaTimeline.CurrentTimeInvalidated += MediaTimelineTimeChangedHandler;
            this.mediaTimeline.Completed += MediaTimeline_Completed;
        }

        private void MediaTimeline_Completed(object sender, EventArgs e)
        {
            MediaController.Pause();
            IsPlaying = false;
        }

        public static readonly RoutedEvent VideoOpenedEvent =
            EventManager.RegisterRoutedEvent("VideoOpened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VideoPlayer));

        public static readonly RoutedEvent IsPlayingChangedEvent =
            EventManager.RegisterRoutedEvent("IsPlayingChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VideoPlayer));

        public static readonly DependencyProperty VideoLengthProperty =
            DependencyProperty.Register(
                "VideoLength",
                typeof(TimeSpan),
                typeof(VideoPlayer),
                new PropertyMetadata(TimeSpan.FromSeconds(10)));

        public static readonly DependencyProperty VideoPositionProperty =
            DependencyProperty.Register(
                "VideoPosition",
                typeof(TimeSpan),
                typeof(VideoPlayer),
                new FrameworkPropertyMetadata(TimeSpan.Zero, VideoPositionPropertyChangedCallback));

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
                return (TimeSpan)GetValue(VideoLengthProperty);
            }
        }

        public TimeSpan VideoPosition
        {
            get
            {
                return (TimeSpan)GetValue(VideoPositionProperty);
            }
            set
            {
                SetValue(VideoPositionProperty, value);
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

        public void OpenFile(string fileName)
        {
            this.mediaTimeline.Source = new Uri(fileName);

            MediaClock clock = this.mediaTimeline.CreateClock();
            clock.Controller.Pause();
            this.mediaElement.Clock = clock;
        }

        public void PlayOrPause()
        {
            PlayOrPause(TimeSpan.Zero, VideoLength);
        }

        public void PlayOrPause(TimeSpan from, TimeSpan to)
        {
            if (!this.isVideoLoaded)
            {
                return;
            }

            if (IsPlaying)
            {
                MediaController.Pause();
                IsPlaying = false;
            }
            else
            {
                if (CurrentMediaTime < from || CurrentMediaTime >= to)
                {
                    MediaController.Seek(from, TimeSeekOrigin.BeginTime);
                }

                this.endOfPlayback = to;

                MediaController.Resume();
                IsPlaying = true;
            }
        }

        public void Stop()
        {
            this.mediaElement.Stop();
            IsPlaying = false;
            SetValue(VideoPositionProperty, TimeSpan.Zero);
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

        private MediaTimeline mediaTimeline;
        private bool isPlaying;
        private TimeSpan endOfPlayback = TimeSpan.MaxValue;
        private bool isVideoLoaded = false;
        private bool positionSetByTimer;

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
            }
        }

        private TimeSpan CurrentMediaTime
        {
            get
            {
                return this.mediaElement.Clock.CurrentTime.Value;
            }
        }

        private ClockController MediaController
        {
            get
            {
                return this.mediaElement.Clock.Controller;
            }
        }

        private static void VideoPositionPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
            {
                return;
            }

            VideoPlayer videoPlayer = (VideoPlayer)d;
            TimeSpan newValue = (TimeSpan)e.NewValue;

            if (!videoPlayer.isVideoLoaded || videoPlayer.positionSetByTimer)
            {
                return;
            }

            videoPlayer.SetValue(VideoPositionProperty, newValue);

            videoPlayer.MediaController.Seek(newValue, TimeSeekOrigin.BeginTime);
        }

        private static void VolumePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VideoPlayer videoPlayer = (VideoPlayer)d;
            double newValue = (double)e.NewValue;

            videoPlayer.mediaElement.Volume = newValue;
            videoPlayer.SetValue(VolumeProperty, newValue);
        }


        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            this.isVideoLoaded = true;
            IsPlaying = false;
            SetValue(VideoLengthProperty, this.mediaElement.NaturalDuration.TimeSpan);
            SetValue(VolumeProperty, this.mediaElement.Volume);
            this.endOfPlayback = TimeSpan.MaxValue;

            RaiseVideoOpenedEvent();
        }

        private void MediaTimelineTimeChangedHandler(object sender, EventArgs e)
        {
            if (CurrentMediaTime >= this.endOfPlayback)
            {
                MediaController.Pause();
                IsPlaying = false;
            }

            this.positionSetByTimer = true;
            SetValue(VideoPositionProperty, CurrentMediaTime);
            this.positionSetByTimer = false;
        }
    }
}
