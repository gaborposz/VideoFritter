using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace VideoFritter.VideoSlice
{
    public class VideoSliceViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TimeSpan SliceStart
        {
            get
            {
                return this.sliceStart;
            }

            set
            {
                this.sliceStart = value;
                OnPropertyChanged();
            }
        }


        public TimeSpan SliceEnd
        {
            get
            {
                return this.sliceEnd;
            }

            set
            {
                this.sliceEnd = value;
                OnPropertyChanged();
            }
        }

        public void Export(string originalVideoFile, string newVideoFile)
        {
            Process ffmpegProc = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal,
                WorkingDirectory = @".\ffmpeg",
                FileName = "ffmpeg.exe",
                Arguments = $"-i {originalVideoFile} -ss {SliceStart} -t {SliceEnd - SliceStart} -vcodec copy -acodec copy {newVideoFile}"
            };
            ffmpegProc.StartInfo = startInfo;
            ffmpegProc.Start();
            ffmpegProc.WaitForExit();
        }

        private TimeSpan sliceStart;
        private TimeSpan sliceEnd;

        private void OnPropertyChanged([CallerMemberName]string caller = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }
    }
}
