using System;

namespace VideoFritter.ProcessingQueue
{
    internal class ProcessingItem
    {
        public ProcessingItem(string fileName, TimeSpan sliceStart, TimeSpan sliceEnd)
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
            return $"FileName: {FileName} Start: {SliceStart:hh\\:mm\\:ss\\.fff} End: {SliceEnd:hh\\:mm\\:ss\\.fff}";
        }
    }
}
