using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

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

            this.positionUpdateTimer = new DispatcherTimer();
            this.positionUpdateTimer.Interval = TimeSpan.FromMilliseconds(10);
            this.positionUpdateTimer.Tick += PositionUpdateTimer_Tick;
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

                if (this.isPlaying)
                {
                    this.positionUpdateTimer.Start();
                }
                else
                {
                    this.positionUpdateTimer.Stop();
                }

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
            // Close the previous video
            this.mediaElement.LoadedBehavior = MediaState.Close;

            // Open and pause the new
            this.mediaElement.Source = new Uri(fileName);
            PauseInternal();
        }

        public void PlayOrPause()
        {
            if (!IsVideoLoaded)
            {
                return;
            }

            if (IsPlaying)
            {
                PauseInternal();
            }
            else
            {
                if (VideoPosition >= VideoLength)
                {
                    // Do nothing if the video is at its end
                    return;
                }

                this.endOfPlayback = TimeSpan.MaxValue;

                PlayInternal(VideoPosition);
            }
        }

        public void Play(TimeSpan from, TimeSpan to)
        {
            if (!IsVideoLoaded)
            {
                return;
            }

            this.endOfPlayback = to;

            PlayInternal(from);
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

        private bool isPlaying;
        private TimeSpan endOfPlayback = TimeSpan.MaxValue;
        private bool positionSetByTimer;
        private DispatcherTimer positionUpdateTimer;

        private static void VideoPositionPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
            {
                return;
            }

            VideoPlayer videoPlayer = (VideoPlayer)d;

            if (videoPlayer.IsVideoLoaded && !videoPlayer.positionSetByTimer)
            {
                if (videoPlayer.IsPlaying)
                {
                    // WORKAROUND: If the slider was changed during playback, then the playback must be stopped, 
                    // otherwise the continous position changed events will overload the player
                    videoPlayer.PlayOrPause();
                }

                TimeSpan newValue = (TimeSpan)e.NewValue;
                videoPlayer.Seek(newValue);
            }
        }

        private static void VolumePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VideoPlayer videoPlayer = (VideoPlayer)d;
            double newValue = (double)e.NewValue;

            videoPlayer.mediaElement.Volume = newValue;
            videoPlayer.SetValue(VolumeProperty, newValue);
        }


        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            IsVideoLoaded = true;
            SetValue(VideoLengthProperty, this.mediaElement.NaturalDuration.HasTimeSpan ? this.mediaElement.NaturalDuration.TimeSpan : TimeSpan.Zero);
            SetValue(VolumeProperty, this.mediaElement.Volume);
            this.endOfPlayback = TimeSpan.MaxValue;

            RaiseVideoOpenedEvent();
            RaiseVideoPositionChangedEvent();
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine($"Media Ended.");

            PauseInternal();
        }

        private void PositionUpdateTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan currentVideoPosition = this.mediaElement.Position;

            if (currentVideoPosition == TimeSpan.Zero)
            {
                // This value is received only due to a bug in MediaElement when the video reached its end, so it can be ignored.
                return;
            }

            if (currentVideoPosition >= this.endOfPlayback)
            {
                PauseInternal();

                Seek(this.endOfPlayback);
            }
            else
            {
                UpdateVideoPositionInternally(currentVideoPosition);
            }
        }

        private void Seek(TimeSpan newPosition)
        {
            TimeSpan currentPosition = this.mediaElement.Position;
            if (currentPosition != newPosition)
            {
                //Debug.WriteLine($"Seeking to {newPosition} from {currentPosition}.");

                this.mediaElement.Position = newPosition;
                UpdateVideoPositionInternally(newPosition);
            }
        }

        private void UpdateVideoPositionInternally(TimeSpan currentVideoPosition)
        {
            //Debug.WriteLine($"VideoPosition was updated to: {currentVideoPosition}");

            this.positionSetByTimer = true;
            SetValue(VideoPositionProperty, currentVideoPosition);
            this.positionSetByTimer = false;

            RaiseVideoPositionChangedEvent();
        }

        private void PlayInternal(TimeSpan startFrom)
        {
            this.mediaElement.LoadedBehavior = MediaState.Play;
            IsPlaying = true;

            // WORKAROUND: When playing is started the Position is sporadically reset to 00:00:00,
            // even if it was seeked previously to a different value.
            // To fix this problem we set the position right after Play, 
            // but to avoid unnecessary twitches we do it only if there is a significant difference (at least 100ms).
            if (Math.Abs(this.mediaElement.Position.TotalMilliseconds - startFrom.TotalMilliseconds) > 100)
            {
                this.mediaElement.Position = startFrom;
            }
        }

        private void PauseInternal()
        {
            this.mediaElement.LoadedBehavior = MediaState.Pause;
            IsPlaying = false;
        }
    }
}
