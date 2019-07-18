using System.Collections.ObjectModel;
using CommonHelpers.Common;
using Xamarin.Forms;
using XkcdViewer.Forms.Common;
using XkcdViewer.Forms.Models;

namespace XkcdViewer.Forms.ViewModels
{
    public class FavoritesPageViewModel : ViewModelBase
    {
        private Command removeFavoritesCommand;
        private ObservableCollection<Comic> selectedFavorites;

        public FavoritesPageViewModel()
        {
            Title = "Favorites";
        }
        
        public ObservableCollection<Comic> FavoriteComics => FavoritesManager.Current.Favorites;

        public ObservableCollection<Comic> SelectedFavorites
        {
            get => selectedFavorites ?? ( selectedFavorites = new ObservableCollection<Comic>());
            set => SetProperty(ref selectedFavorites, value);
        }

        public Command RemoveFavoritesCommand => removeFavoritesCommand ?? (removeFavoritesCommand = new Command((comic) =>
        {
            for (int i = 0; i <= SelectedFavorites.Count - 1; i++)
            {
                FavoriteComics.Remove(SelectedFavorites[i]);

                // Since we may be removing several items, we don't want to trigger a save until the last item
                if (i == SelectedFavorites.Count - 1)
                {
                    // passing "False" prevents a save operation
                    FavoritesManager.Current.RemoveFavorite(SelectedFavorites[i], false);
                }
                else
                {
                    // the last item in the list triggers a save to disk operation
                    FavoritesManager.Current.RemoveFavorite(SelectedFavorites[i]);
                }
            }
        }));
    }
}