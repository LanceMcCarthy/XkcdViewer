using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Portable.Annotations;
using Portable.Models;
using Portable.Views;
using Xamarin.Forms;

namespace Portable.ViewModels
{
    public class FavoritesPageViewModel : INotifyPropertyChanged
    {
        private Command<Comic> loadDetailsCommand;
        private Command removeFavoritesCommand;
        private Command<Comic> shareCommand;
        private ObservableCollection<Comic> selectedFavorites;

        public FavoritesPageViewModel()
        {
            
        }

        public ObservableCollection<Comic> FavoriteComics => App.ViewModel.FavoriteComics;

        public ObservableCollection<Comic> SelectedFavorites
        {
            get { return selectedFavorites ?? ( selectedFavorites = new ObservableCollection<Comic>()); }
            set { selectedFavorites = value; OnPropertyChanged();}
        }

        #region Commands
        
        public Command<Comic> LoadDetailsCommand => loadDetailsCommand ?? (loadDetailsCommand = new Command<Comic>((comic) =>
        {
            if (comic == null)
                return;

            var detailsPage = new DetailsPage();
            var dpvm = detailsPage.BindingContext as DetailsPageViewModel;

            if (dpvm != null)
                dpvm.SelectedComic = comic;

            App.RootPage.Navigation.PushAsync(detailsPage);

        }));

        public Command RemoveFavoritesCommand => removeFavoritesCommand ?? (removeFavoritesCommand = new Command(async (comic) =>
        {
            foreach (var selectedFavorite in SelectedFavorites)
            {
                await App.ViewModel.RemoveFavoriteAsync(selectedFavorite);
            }
            
        }));

        public Command<Comic> ShareCommand => shareCommand ?? (shareCommand = new Command<Comic>((comic) =>
        {
            if (comic == null) return;
            Debug.WriteLine($"ShareCommand fired - SelectedComic: {comic.Title}");
        }));


        #endregion

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
