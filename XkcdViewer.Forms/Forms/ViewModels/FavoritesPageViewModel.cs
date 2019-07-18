using System.Collections.ObjectModel;
using CommonHelpers.Common;
using XkcdViewer.Forms.Common;
using XkcdViewer.Forms.Models;

namespace XkcdViewer.Forms.ViewModels
{
    public class FavoritesPageViewModel : ViewModelBase
    {
        public FavoritesPageViewModel()
        {
            Title = "Favorites";
        }
        
        public ObservableCollection<Comic> FavoriteComics => FavoritesManager.Current.Favorites;
    }
}