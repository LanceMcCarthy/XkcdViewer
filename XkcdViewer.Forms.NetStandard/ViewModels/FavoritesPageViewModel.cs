using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Telerik.XamarinForms.DataControls.ListView;
using Xamarin.Forms;
using XkcdViewer.Forms.NetStandard.Common;
using XkcdViewer.Forms.NetStandard.Models;
using XkcdViewer.Forms.NetStandard.Views;

namespace XkcdViewer.Forms.NetStandard.ViewModels
{
    public class FavoritesPageViewModel : PageBaseViewModel
    {
        private readonly NavigationService navigationService;

        private ObservableCollection<Comic> favoriteComics;
        private Command loadDetailsCommand;
        private Command removeFavoritesCommand;
        private Command<Comic> shareCommand;
        private ObservableCollection<Comic> selectedFavorites;

        public FavoritesPageViewModel(NavigationService navService)
        {
            this.navigationService = navService;

            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            FavoriteComics = FileHelpers.LoadFavorites();
        }

        public ObservableCollection<Comic> FavoriteComics
        {
            get => favoriteComics ?? (favoriteComics = new ObservableCollection<Comic>());
            set => Set(ref favoriteComics, value);
        }

        public ObservableCollection<Comic> SelectedFavorites
        {
            get => selectedFavorites ?? ( selectedFavorites = new ObservableCollection<Comic>());
            set => Set(ref selectedFavorites, value);
        }

        #region Commands
        
        public Command LoadDetailsCommand => loadDetailsCommand ?? (loadDetailsCommand = new Command(args =>
        {
            if ((args as ItemTapEventArgs)?.Item is Comic comic)
            {
                navigationService.Navigate(typeof(DetailsPage), comic);
            }
        }));

        public Command RemoveFavoritesCommand => removeFavoritesCommand ?? (removeFavoritesCommand = new Command(async (comic) =>
        {
            foreach (var selectedFavorite in SelectedFavorites)
            {
                FavoriteComics.Remove(selectedFavorite);
            }

            FileHelpers.SaveFavorites(FavoriteComics);
        }));

        public Command<Comic> ShareCommand => shareCommand ?? (shareCommand = new Command<Comic>((comic) =>
        {
            if (comic == null) return;
            Debug.WriteLine($"ShareCommand fired - SelectedComic: {comic.Title}");
        }));


        #endregion
        

        public bool RemoveFavorite(Comic comic)
        {
            try
            {
                FavoriteComics.Remove(comic);
                FileHelpers.SaveFavorites(FavoriteComics);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RemoveFavoriteAsync Exception: {ex}");
                return false;
            }
        }

        public bool AddFavorite(Comic comic)
        {
            try
            {
                FavoriteComics.Add(comic);
                return FileHelpers.SaveFavorites(FavoriteComics);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AddFavoriteAsync Exception: {ex}");
                return false;
            }
        }
    }
}