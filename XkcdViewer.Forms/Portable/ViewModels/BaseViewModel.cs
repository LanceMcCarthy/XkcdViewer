using System.Threading.Tasks;
using Cimbalino.Toolkit.Handlers;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight;

namespace Portable.ViewModels
{
    public abstract class BaseViewModel : ViewModelBase
    {
    }

    public abstract class PageBaseViewModel : BaseViewModel, IHandleNavigatedTo, IHandleNavigatedFrom
    {
        private bool isBusy;
        private string isBusyMessage;

        public bool IsBusy
        {
            get { return isBusy; }
            set { Set(ref isBusy, value); }
        }

        public string IsBusyMessage
        {
            get { return isBusyMessage; }
            set { Set(ref isBusyMessage, value); }
        }

        public virtual Task OnNavigatedToAsync(NavigationServiceNavigationEventArgs eventArgs)
        {
            return Task.FromResult(0);
        }

        public virtual Task OnNavigatedFromAsync(NavigationServiceNavigationEventArgs eventArgs)
        {
            return Task.FromResult(0);
        }
    }
}
