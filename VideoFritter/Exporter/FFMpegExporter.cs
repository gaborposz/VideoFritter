using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace VideoFritter.Exporter
{
    internal class FFMpegExporter
    {
        public string GenerateFileName(string originalFileName, string targetPath)
        {
            int counter = 1;
            string exportedVideoFileName;

            do
            {
                exportedVideoFileName = Path.Combine(
                    targetPath,
                    $"{Path.GetFileNameWithoutExtension(originalFileName)}_{counter}{Path.GetExtension(originalFileName)}");
                counter++;
            } while (File.Exists(exportedVideoFileName));

            return exportedVideoFileName;
        }

        public void Export(string sourceFileName, string targetFileName, TimeSpan sliceStart, TimeSpan sliceEnd)
        {
            if (string.IsNullOrWhiteSpace(sourceFileName))
            {
                throw new ArgumentNullException(nameof(sourceFileName), "The source file name cannot be null or empty!");
            }

            if (string.IsNullOrWhiteSpace(targetFileName))
            {
                throw new ArgumentNullException(nameof(sourceFileName), "The source file name cannot be null or empty!");
            }

            string targetDirectory = Path.GetDirectoryName(targetFileName);
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            string ffmpegPath = Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg", "ffmpeg.exe");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = $"-i {sourceFileName} -ss {sliceStart} -t {sliceEnd - sliceStart} -vcodec copy -acodec copy {targetFileName}",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            Process ffmpegProc = new Process
            {
                StartInfo = startInfo
            };

            ffmpegProc.Start();
            ffmpegProc.WaitForExit();

            if (ffmpegProc.ExitCode != 0)
            {
                MessageBox.Show(ffmpegProc.StandardError.ReadToEnd());
            }
        }
    }
}
