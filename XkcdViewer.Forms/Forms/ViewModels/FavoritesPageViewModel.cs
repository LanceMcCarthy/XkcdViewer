using System.Collections.ObjectModel;
using System.Diagnostics;
using CommonHelpers.Common;
using Xamarin.Forms;
using XkcdViewer.Forms.Common;
using XkcdViewer.Forms.Models;

namespace XkcdViewer.Forms.ViewModels
{
    public class FavoritesPageViewModel : ViewModelBase
    {
        private Command removeFavoritesCommand;
        private Command<Comic> shareCommand;
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
            for (int i = 0; i < SelectedFavorites.Count - 1; i++)
            {
                FavoriteComics.Remove(SelectedFavorites[i]);

                // Since we may be removing several items, we dont want to trigger a save until all of the items have been removed
                if (i == SelectedFavorites.Count - 1)
                {
                    // passing "False" prevents a save operation
                    FavoritesManager.Current.RemoveFavorite(SelectedFavorites[i], false);
                }
                else
                {
                    FavoritesManager.Current.RemoveFavorite(SelectedFavorites[i]);
                }
            }
        }));

        public Command<Comic> ShareCommand => shareCommand ?? (shareCommand = new Command<Comic>((comic) =>
        {
            if (comic == null) return;
            Debug.WriteLine($"ShareCommand fired - SelectedComic: {comic.Title}");
        }));
    }
}