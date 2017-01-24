using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Plugin.Share;
using Plugin.Share.Abstractions;
using Portable.Annotations;
using Portable.Models;
using Xamarin.Forms;

namespace Portable.ViewModels
{
    public class DetailsPageViewModel : INotifyPropertyChanged
    {
        private Comic selectedComic;
        private bool isFavorite;
        private Command<Comic> toggleFavoriteCommand;
        private Command<Comic> shareCommand;

        public DetailsPageViewModel()
        {
            if (ViewModelLocator.IsDesignTime)
            {
                SelectedComic = new Comic()
                {
                    Title = "Selected Comic!"
                };
            }
        }

        public Comic SelectedComic
        {
            get { return selectedComic; }
            set { selectedComic = value; OnPropertyChanged(); }
        }

        public bool IsFavorite
        {
            get
            {
                isFavorite = App.ViewModel.FavoriteComics.Contains(selectedComic);
                return isFavorite;
            }
            set { isFavorite = value; OnPropertyChanged(); }
        }

        public Command<Comic> ToggleFavoriteCommand => toggleFavoriteCommand ?? (toggleFavoriteCommand = new Command<Comic>(async (comic) =>
      {
          if (IsFavorite)
          {
              if (await App.ViewModel.RemoveFavoriteAsync(comic))
                  IsFavorite = false; //if removing the fav was successful, update current state
           }
          else
          {
              if (await App.ViewModel.AddFavoriteAsync(comic))
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

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
