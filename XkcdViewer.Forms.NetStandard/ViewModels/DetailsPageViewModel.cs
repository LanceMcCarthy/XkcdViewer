using System.Diagnostics;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using Plugin.Share;
using Plugin.Share.Abstractions;
using Xamarin.Forms;
using XkcdViewer.Forms.NetStandard.Common;
using XkcdViewer.Forms.NetStandard.Models;

namespace XkcdViewer.Forms.NetStandard.ViewModels
{
    public class DetailsPageViewModel : PageBaseViewModel
    {
        private readonly NavigationService navigationService;
        private readonly FavoritesPageViewModel favoritesViewModel;

        private Comic selectedComic;
        private bool isFavorite;
        private Command<Comic> toggleFavoriteCommand;
        private Command<Comic> shareCommand;

        public DetailsPageViewModel(NavigationService navService, FavoritesPageViewModel favsViewModel)
        {
            this.navigationService = navService;
            this.favoritesViewModel = favsViewModel;
        }

        public Comic SelectedComic
        {
            get => selectedComic;
            set
            {
                Set(ref selectedComic, value);
                IsFavorite = favoritesViewModel.FavoriteComics.Contains(selectedComic);
            }
        }

        public bool IsFavorite
        {
            get => isFavorite;
            set => Set(ref isFavorite, value);
        }

        public Command<Comic> ToggleFavoriteCommand => toggleFavoriteCommand ?? (toggleFavoriteCommand = new Command<Comic>(async (comic) =>
        {
            if (IsFavorite)
            {
                if (favoritesViewModel.RemoveFavorite(comic))
                    IsFavorite = false; //if removing the fav was successful, update current state
            }
            else
            {
                if (favoritesViewModel.AddFavorite(comic))
                    IsFavorite = true; //if adding the fav was successful, update current state
            }
        }));

        public Command<Comic> ShareCommand => shareCommand ?? (shareCommand = new Command<Comic>(async (comic) =>
        {
            if (comic == null)
                return;

            Debug.WriteLine($"ShareCommand fired - SelectedComic: {comic.Title}");

            await CrossShare.Current.Share(
                new ShareMessage
                {
                    Title = comic.Title,
                    Text = comic.Transcript,
                    Url = comic.Img
                },
                new ShareOptions
                {
                    ExcludedUIActivityTypes = new[] { ShareUIActivityType.PostToFacebook }
                });
        }));

        public override Task OnNavigatedToAsync(NavigationServiceNavigationEventArgs eventArgs)
        {
            if (eventArgs?.Parameter != null)
            {
                SelectedComic = eventArgs.Parameter as Comic;
            }

            return base.OnNavigatedToAsync(eventArgs);
        }

        public override Task OnNavigatedFromAsync(NavigationServiceNavigationEventArgs eventArgs)
        {
            return base.OnNavigatedFromAsync(eventArgs);
        }
    }
}