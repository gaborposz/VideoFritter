using System;

namespace VideoFritter.Common
{
    internal class AbstractExportingViewModel : AbstractViewModelBase, IProgress<double>
    {
        public event EventHandler<bool> IsExportingChangedEvent;

        public bool IsExporting
        {
            get
            {
                return this.isExporting;
            }

            protected set
            {
                if (this.isExporting != value)
                {
                    this.isExporting = value;
                    OnPropertyChanged();
                    IsExportingChangedEvent?.Invoke(this, value);
                }
            }
        }

        public double ExportProgress
        {
            get
            {
                return this.exportProgress;
            }

            protected set
            {
                if (this.exportProgress != value)
                {
                    this.exportProgress = value;
                    OnPropertyChanged();
                }
            }
        }

        void IProgress<double>.Report(double value)
        {
            ExportProgress = value;
        }

        private bool isExporting;
        private double exportProgress;

    }
}
