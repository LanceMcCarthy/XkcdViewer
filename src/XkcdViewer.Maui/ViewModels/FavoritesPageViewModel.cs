using System.Collections.ObjectModel;
using CommonHelpers.Common;
using XkcdViewer.Maui.Common;
using XkcdViewer.Maui.Models;

namespace XkcdViewer.Maui.ViewModels;

public class FavoritesPageViewModel : ViewModelBase
{
    public FavoritesPageViewModel()
    {
        Title = "Favorites";
    }
        
    public ObservableCollection<Comic> FavoriteComics => FavoritesManager.Current.Favorites;
}