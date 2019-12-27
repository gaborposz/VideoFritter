using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace VideoFritter.Controls.VideoPlayer
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

        public static readonly RoutedEvent VideoPositionChangedEvent =
            EventManager.RegisterRoutedEvent("VideoPositionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VideoPlayer));


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
                new PropertyMetadata(TimeSpan.Zero, VideoPositionPropertyChangedCallback));

        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register(
                "VideoVolume",
                typeof(double),
                typeof(VideoPlayer),
                new PropertyMetadata(0.5d, VolumePropertyChangedCallback));

        public static readonly DependencyProperty IsVideoLoadedProperty =
            DependencyProperty.Register(
                "IsVideoLoaded",
                typeof(bool),
                typeof(VideoPlayer),
                new PropertyMetadata(false));

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

        public bool IsPlaying
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

        public bool IsVideoLoaded
        {
            get
            {
                return (bool)GetValue(IsVideoLoadedProperty);
            }
            set
            {
                SetValue(IsVideoLoadedProperty, value);
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

        public event RoutedEventHandler VideoPositionChanged
        {
            add { AddHandler(VideoPositionChangedEvent, value); }
            remove { RemoveHandler(VideoPositionChangedEvent, value); }
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
            if (IsPlaying)
            {
                MediaController.Stop();
            }

            this.mediaTimeline.Source = new Uri(fileName);

            MediaClock clock = this.mediaTimeline.CreateClock();
            clock.Controller.Pause();
            this.mediaElement.Clock = clock;
        }

        public void PlayOrPause()
        {
            if (!IsVideoLoaded)
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
                this.endOfPlayback = VideoLength;

                MediaController.Resume();
                IsPlaying = true;
            }
        }

        public void Play(TimeSpan from, TimeSpan to)
        {
            if (!IsVideoLoaded)
            {
                return;
            }

            if (IsPlaying)
            {
                MediaController.Pause();
            }

            if (from != VideoPosition)
            {
                MediaController.Seek(from, TimeSeekOrigin.BeginTime);
            }

            this.endOfPlayback = to;

            MediaController.Resume();
            IsPlaying = true;
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

        protected virtual void RaiseVideoPositionChangedEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(
                VideoPositionChangedEvent,
                this);

            RaiseEvent(args);
        }

        private MediaTimeline mediaTimeline;
        private bool isPlaying;
        private TimeSpan endOfPlayback = TimeSpan.MaxValue;
        private bool positionSetByTimer;

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

            if (!videoPlayer.IsVideoLoaded || videoPlayer.positionSetByTimer)
            {
                return;
            }

            videoPlayer.SetValue(VideoPositionProperty, newValue);

            if (videoPlayer.IsPlaying)
            {
                // WORKAROUND: If the slider was changed during playback, then the playback must be stopped, 
                // otherwise the continous position changed events will overload the player
                videoPlayer.PlayOrPause();
            }

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
            IsVideoLoaded = true;
            IsPlaying = false;
            SetValue(VideoLengthProperty, this.mediaElement.NaturalDuration.HasTimeSpan ? this.mediaElement.NaturalDuration.TimeSpan : TimeSpan.Zero);
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

            RaiseVideoPositionChangedEvent();
        }
    }
}
