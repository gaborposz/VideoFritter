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

                if (this.isExporting)
                {
                    // Start with "indeterminate" progress:
                    ExportProgress = -1;
                    IsIndeterminateProgess = true;
                }
            }
        }

        public bool IsIndeterminateProgess
        {
            get
            {
                return this.isIndeterminateProgess;
            }

            private set
            {
                if (this.isIndeterminateProgess != value)
                {
                    this.isIndeterminateProgess = value;
                    OnPropertyChanged();
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
                    IsIndeterminateProgess = false;
                    OnPropertyChanged();
                }
            }
        }

        void IProgress<double>.Report(double value)
        {
            ExportProgress = value;
        }

        private bool isExporting;
        private bool isIndeterminateProgess;
        private double exportProgress;

    }
}
