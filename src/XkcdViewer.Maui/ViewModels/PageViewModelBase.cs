using CommonHelpers.Common;
using XkcdViewer.Maui.Interfaces;

namespace XkcdViewer.Maui.ViewModels;

public class PageViewModelBase : ViewModelBase, IViewModel
{
    public virtual void OnAppearing() {}

    public virtual bool OnBackButtonRequested() => false;
    
    public virtual void OnNavigatingFrom(NavigatingFromEventArgs args){}

    public virtual void OnNavigatedFrom(NavigatedFromEventArgs args){}

    public virtual void OnNavigatedTo(NavigatedToEventArgs args){}
}