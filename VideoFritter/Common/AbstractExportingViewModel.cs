using System;

namespace VideoFritter.Common
{
    internal class AbstractExportingViewModel : AbstractViewModelBase, IProgress<double>
    {
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
                }

                if (this.isExporting)
                {
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
                    OnPropertyChanged();
                }
            }
        }

        void IProgress<double>.Report(double value)
        {
            if (value > 0)
            {
                IsIndeterminateProgess = false;
            }
            ExportProgress = value;
        }

        private bool isExporting;
        private bool isIndeterminateProgess;
        private double exportProgress;

    }
}
