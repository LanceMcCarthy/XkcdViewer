using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Portable.Annotations;
using Portable.Models;
using Xamarin.Forms;

namespace Portable.ViewModels
{
    public class DetailsPageViewModel : INotifyPropertyChanged
    {
        private Comic selectedComic;
        private bool isFavorite;
        private Command toggleFavoriteCommand;

        public DetailsPageViewModel()
        {
            
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

        public Command ToggleFavoriteCommand => toggleFavoriteCommand ?? (toggleFavoriteCommand = new Command(() =>
        {
            if (IsFavorite)
            {
                App.ViewModel.FavoriteComics.Remove(selectedComic);
                IsFavorite = false;
            }
            else
            {
                App.ViewModel.FavoriteComics.Add(selectedComic);
                IsFavorite = true;
            }
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
