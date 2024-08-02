using System.Collections.ObjectModel;
using CommonHelpers.Models;
using XkcdViewer.Maui.Models;

namespace XkcdViewer.Maui.Interfaces;

public interface IViewModel
{
    void OnAppearing();

    bool OnBackButtonRequested();

    void OnNavigatingFrom(NavigatingFromEventArgs args);

    void OnNavigatedFrom(NavigatedFromEventArgs args);

    void OnNavigatedTo(
        NavigatedToEventArgs args, 
        ObservableCollection<Comic>? favoriteComics);
}
