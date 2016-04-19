using System;
using System.Diagnostics;
using Xamarin.Forms;
using XkcdViewer.Models;

namespace XkcdViewer
{
    public partial class FavoritesPage : ContentPage
    {
        public FavoritesPage()
        {
            InitializeComponent();
            this.BindingContext = App.ViewModel;
        }

        private async void UnfavoriteButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var button = sender as Button;
                var selectedComic = button?.BindingContext as Comic;
                if (selectedComic == null) return;

                if (App.ViewModel.FavoriteComics.Contains(selectedComic))
                {
                    App.ViewModel.FavoriteComics.Remove(selectedComic);
                    await Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UnfavoriteButtonClicked Exception: {ex}");
            }
            finally
            {
                MyRadListView.EndItemSwipe();
            }
        }

        public void OnDeleteFav(object sender, EventArgs e)
        {
            var mi = (MenuItem) sender;
            var comic = mi.CommandParameter as Comic;

            if (App.ViewModel.FavoriteComics.Contains(comic))
            {
                App.ViewModel.FavoriteComics.Remove(comic);
            }
        }
    }
}
