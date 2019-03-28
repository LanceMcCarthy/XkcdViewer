using System.Diagnostics;
using CommonHelpers.Common;
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
        private string pageTitle;

        public DetailsPageViewModel()
        {
        }

        public Comic SelectedComic
        {
            get => selectedComic;
            set
            {
                if (SetProperty(ref selectedComic, value))
                {
                    PageTitle = $"#{SelectedComic.Num}";
                    IsFavorite = FavoritesManager.Current.IsFavorite(SelectedComic);
                }
            }
        }

        public string PageTitle
        {
            get => pageTitle;
            set => SetProperty(ref pageTitle, value);
        }

        public bool IsFavorite
        {
            get => isFavorite;
            set => SetProperty(ref isFavorite, value);
        }

        public Command<Comic> ToggleFavoriteCommand => toggleFavoriteCommand ?? (toggleFavoriteCommand = new Command<Comic>((comic) =>
        {
            if (IsFavorite)
            {
                comic.RemoveFavoriteCommand.Execute(null);
                IsFavorite = false;
            }
            else
            {
                comic.SaveFavoriteCommand.Execute(null);
                IsFavorite = true;
            }
        }));
    }
}