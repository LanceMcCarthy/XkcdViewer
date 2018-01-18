using System.Threading.Tasks;
using Cimbalino.Toolkit.Handlers;
using Cimbalino.Toolkit.Services;
using XkcdViewer.Forms.NetStandard.Common;

namespace XkcdViewer.Forms.NetStandard.ViewModels
{
    public abstract class PageBaseViewModel : ObservableObject, IHandleNavigatedTo, IHandleNavigatedFrom
    {
        private bool isBusy;
        private string isBusyMessage;

        public bool IsBusy
        {
            get => isBusy;
            set => Set(ref isBusy, value);
        }

        public string IsBusyMessage
        {
            get => isBusyMessage;
            set => Set(ref isBusyMessage, value);
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
