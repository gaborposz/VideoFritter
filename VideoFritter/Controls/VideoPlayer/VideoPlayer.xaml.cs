using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

using FFmpegWrapper;

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

            this.mediaPlayer = new MediaPlayer();
            this.mediaPlayer.ScrubbingEnabled = true;
            this.mediaPlayer.MediaOpened += MediaOpenedHandler;
            this.mediaPlayer.MediaEnded += MediaEndedHandler;

            VideoDrawing drawing = new VideoDrawing();
            drawing.Rect = new Rect(0, 0, 100, 100);
            drawing.Player = this.mediaPlayer;
            DrawingBrush brush = new DrawingBrush(drawing);
            this.videoCanvas.Background = brush;
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
                return this.mediaPlayer.NaturalVideoWidth;
            }
        }

        public int VideoHeight
        {
            get
            {
                return this.mediaPlayer.NaturalVideoHeight;
            }
        }

        public void OpenFile(string fileName)
        {
            this.videoCanvas.Visibility = Visibility.Hidden;


            // WORKAROUND: The MediaPlayer component sets the length inprecisely, 
            // so we have to get it directly from the file
            TimeSpan realLength;
            using (InputMediaFile inputFile = new InputMediaFile(fileName))
            {
                realLength = inputFile.Length;
            }

            SetValue(VideoLengthProperty, realLength);

            this.timeline = new MediaTimeline(new Uri(fileName, UriKind.Absolute));
            this.timeline.Duration = new Duration(realLength);

            this.clock = this.timeline.CreateClock();
            this.clock.CurrentTimeInvalidated += TimeInvalidatedHandler;

            this.mediaPlayer.Clock = this.clock;
            this.mediaPlayer.Clock.Completed += MediaEndedHandler;

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
                this.timeline.Duration = VideoLength;

                PlayInternal();
            }
        }

        public void Play(TimeSpan from, TimeSpan to)
        {
            if (!IsVideoLoaded)
            {
                return;
            }

            this.endOfPlayback = to;

            SeekInternal(from);
            PlayInternal();
        }

        protected virtual void RaiseVideoOpenedEvent()
        {
            VideoOpenedEventArgs args = new VideoOpenedEventArgs(
                VideoOpenedEvent,
                this,
                VideoWidth,
                VideoHeight);

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

        private MediaPlayer mediaPlayer;
        private MediaTimeline timeline;
        private MediaClock clock;
        private bool isPlaying;
        private TimeSpan endOfPlayback = TimeSpan.MaxValue;
        private bool positionWasSetInternally;

        private ClockController Controller
        {
            get
            {

                return this.clock.Controller;
            }
        }


        private static void VideoPositionPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
            {
                return;
            }

            VideoPlayer videoPlayer = (VideoPlayer)d;

            if (videoPlayer.IsVideoLoaded && !videoPlayer.positionWasSetInternally)
            {
                if (videoPlayer.IsPlaying)
                {
                    // WORKAROUND: If the slider was changed during playback, then the playback must be stopped, 
                    // otherwise the continous position change events will overload the player
                    videoPlayer.PlayOrPause();
                }

                TimeSpan newValue = (TimeSpan)e.NewValue;
                videoPlayer.SeekInternal(newValue);
            }
        }

        private static void VolumePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VideoPlayer videoPlayer = (VideoPlayer)d;
            double newValue = (double)e.NewValue;

            videoPlayer.mediaPlayer.Volume = newValue;
            videoPlayer.SetValue(VolumeProperty, newValue);
        }

        private void VideoPlayer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateVideoSize(e.NewSize.Width, e.NewSize.Height);
        }

        private void MediaOpenedHandler(object sender, EventArgs e)
        {
            PauseInternal();

            IsVideoLoaded = true;
            this.endOfPlayback = TimeSpan.MaxValue;

            SetValue(VolumeProperty, this.mediaPlayer.Volume);

            RaiseVideoOpenedEvent();
            RaiseVideoPositionChangedEvent();

            UpdateVideoSize(this.ActualWidth, this.ActualHeight);

            this.videoCanvas.Visibility = Visibility.Visible;
        }

        private void MediaEndedHandler(object sender, EventArgs e)
        {
            PauseInternal();

            // WORKAROUND: The clock's CurrentTime is lagging behind the real position, 
            // so when the video playback is finished it remains sligthly before the 'real end'.
            // To hide it we need to set the position to the 'real end' manually.
            UpdateVideoPositionInternally(VideoLength);
        }

        private void TimeInvalidatedHandler(object sender, EventArgs e)
        {
            TimeSpan currentVideoPosition = this.clock.CurrentTime.Value;

            if (currentVideoPosition == TimeSpan.Zero)
            {
                // This value is received only due to a bug in MediaElement when the video reached its end, so it can be ignored.
                return;
            }

            if (IsPlaying)
            {
                if (currentVideoPosition >= this.endOfPlayback)
                {
                    PauseInternal();

                    // WORKAROUND: The clock's CurrentTime is lagging behind the real position, 
                    // so when the video playback is finished it remains sligthly before the 'real end'.
                    // To hide it we need to set the position to the 'real end' manually.
                    UpdateVideoPositionInternally(this.endOfPlayback);
                }
                else
                {
                    UpdateVideoPositionInternally(currentVideoPosition);
                }
            }
        }


        private void UpdateVideoPositionInternally(TimeSpan currentVideoPosition)
        {
            this.positionWasSetInternally = true;
            SetValue(VideoPositionProperty, currentVideoPosition);
            this.positionWasSetInternally = false;

            RaiseVideoPositionChangedEvent();
        }

        private void PlayInternal()
        {
            Controller.Resume();
            IsPlaying = true;
        }

        private void SeekInternal(TimeSpan newPosition)
        {
            Controller.Seek(newPosition, TimeSeekOrigin.BeginTime);
            UpdateVideoPositionInternally(newPosition);
        }

        private void PauseInternal()
        {
            Controller.Pause();
            IsPlaying = false;
        }

        private void UpdateVideoSize(double availableWidth, double availableHeight)
        {
            if (availableWidth == 0 || availableHeight == 0 || VideoWidth == 0 || VideoHeight == 0)
            {
                return;
            }

            double widthRatio = VideoWidth / availableWidth;
            double heightRatio = VideoHeight / availableHeight;

            if (widthRatio > heightRatio)
            {
                this.videoCanvas.Width = availableWidth;
                this.videoCanvas.Height = VideoHeight / widthRatio;
            }
            else
            {
                this.videoCanvas.Width = VideoWidth / heightRatio;
                this.videoCanvas.Height = availableHeight;
            }
        }
    }
}
