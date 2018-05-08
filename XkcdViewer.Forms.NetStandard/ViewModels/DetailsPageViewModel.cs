using System.Diagnostics;
using Plugin.Share;
using Plugin.Share.Abstractions;
using Xamarin.Forms;
using XkcdViewer.Forms.NetStandard.Common;
using XkcdViewer.Forms.NetStandard.Models;

namespace XkcdViewer.Forms.NetStandard.ViewModels
{
    public class DetailsPageViewModel : ViewModelBase
    {
        private Comic selectedComic;
        private bool isFavorite;
        private Command<Comic> toggleFavoriteCommand;
        private Command<Comic> shareCommand;

        public DetailsPageViewModel(Comic comic)
        {
            SelectedComic = comic;
            IsFavorite = FavoritesManager.Current.IsFavorite(SelectedComic);
        }

        public Comic SelectedComic
        {
            get => selectedComic;
            set => Set(ref selectedComic, value);
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
                FavoritesManager.Current.RemoveFavorite(comic);
                IsFavorite = false;
            }
            else
            {
                FavoritesManager.Current.AddFavorite(comic);
                IsFavorite = true;
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
        
    }
}