using System.Collections.ObjectModel;
using System.Diagnostics;
using Telerik.XamarinForms.DataControls.ListView;
using Xamarin.Forms;
using XkcdViewer.Forms.NetStandard.Common;
using XkcdViewer.Forms.NetStandard.Models;
using XkcdViewer.Forms.NetStandard.Views;

namespace XkcdViewer.Forms.NetStandard.ViewModels
{
    public class FavoritesPageViewModel : ViewModelBase
    {
        private Command removeFavoritesCommand;
        private Command<Comic> shareCommand;
        private ObservableCollection<Comic> selectedFavorites;

        public FavoritesPageViewModel()
        {

        }
        
        public ObservableCollection<Comic> FavoriteComics => FavoritesManager.Current.Favorites;

        public ObservableCollection<Comic> SelectedFavorites
        {
            get => selectedFavorites ?? ( selectedFavorites = new ObservableCollection<Comic>());
            set => Set(ref selectedFavorites, value);
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