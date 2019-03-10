using System;

namespace VideoFritter.ExportQueue
{
    internal class ExportItem
    {
        public ExportItem(string fileName, TimeSpan sliceStart, TimeSpan sliceEnd)
        {
            FileName = fileName;
            SliceStart = sliceStart;
            SliceEnd = sliceEnd;
        }

        public string FileName { get; }
        public TimeSpan SliceStart { get; }
        public TimeSpan SliceEnd { get; }

        public override string ToString()
        {
            return $"{FileName}     {SliceStart:hh\\:mm\\:ss\\.fff} - {SliceEnd:hh\\:mm\\:ss\\.fff}";
        }
    }
}
