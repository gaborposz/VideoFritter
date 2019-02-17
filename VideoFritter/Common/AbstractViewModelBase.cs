using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VideoFritter.Common
{
    internal abstract class AbstractViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string caller = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }
    }
}
