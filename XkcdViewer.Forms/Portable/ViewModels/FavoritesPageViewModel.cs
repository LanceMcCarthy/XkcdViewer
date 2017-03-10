using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Portable.Common;
using Portable.Models;
using Portable.Views;
using Xamarin.Forms;

namespace Portable.ViewModels
{
    public class FavoritesPageViewModel : PageBaseViewModel
    {
        private readonly NavigationService navigationService;

        private ObservableCollection<Comic> favoriteComics;
        private Command<Comic> loadDetailsCommand;
        private Command removeFavoritesCommand;
        private Command<Comic> shareCommand;
        private ObservableCollection<Comic> selectedFavorites;

        public FavoritesPageViewModel(NavigationService navService)
        {
            this.navigationService = navService;

            InitializeViewModel();
        }

        private async void InitializeViewModel()
        {
            FavoriteComics = await FileHelpers.LoadFavoritesAsync();
        }

        public ObservableCollection<Comic> FavoriteComics
        {
            get { return favoriteComics ?? (favoriteComics = new ObservableCollection<Comic>()); }
            set { favoriteComics = value; }
        }

        public ObservableCollection<Comic> SelectedFavorites
        {
            get { return selectedFavorites ?? ( selectedFavorites = new ObservableCollection<Comic>()); }
            set { Set(ref selectedFavorites, value); }
        }

        #region Commands
        
        public Command<Comic> LoadDetailsCommand => loadDetailsCommand ?? (loadDetailsCommand = new Command<Comic>((comic) =>
        {
            if (comic == null)
                return;

            //var detailsPage = new DetailsPage();
            //var dpvm = detailsPage.BindingContext as DetailsPageViewModel;

            //if (dpvm != null)
            //    dpvm.SelectedComic = comic;

            //App.RootPage.Navigation.PushAsync(detailsPage);

            this.navigationService.Navigate(typeof(DetailsPage), comic);

        }));

        public Command RemoveFavoritesCommand => removeFavoritesCommand ?? (removeFavoritesCommand = new Command(async (comic) =>
        {
            foreach (var selectedFavorite in SelectedFavorites)
            {
                FavoriteComics.Remove(selectedFavorite);
            }

            await FileHelpers.SaveFavoritesAsync(FavoriteComics);

        }));

        public Command<Comic> ShareCommand => shareCommand ?? (shareCommand = new Command<Comic>((comic) =>
        {
            if (comic == null) return;
            Debug.WriteLine($"ShareCommand fired - SelectedComic: {comic.Title}");
        }));


        #endregion
        

        public async Task<bool> RemoveFavoriteAsync(Comic comic)
        {
            try
            {
                FavoriteComics.Remove(comic);
                await FileHelpers.SaveFavoritesAsync(FavoriteComics);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RemoveFavoriteAsync Exception: {ex}");
                return false;
            }
        }

        public async Task<bool> AddFavoriteAsync(Comic comic)
        {
            try
            {
                FavoriteComics.Add(comic);
                await FileHelpers.SaveFavoritesAsync(FavoriteComics);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AddFavoriteAsync Exception: {ex}");
                return false;
            }
        }
    }
}