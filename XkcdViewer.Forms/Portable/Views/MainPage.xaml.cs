using System;
using Portable.Models;
using Portable.ViewModels;
using Portable.Views;
using Telerik.XamarinForms.DataControls.ListView;
using Xamarin.Forms;

namespace Portable.Views
{
    public partial class MainPage : ContentPage
    {
        private bool isUsingSecondTemplate = false;

        public MainPage()
        {
            InitializeComponent();
        }

        //private async void GetNextComicButton_OnClicked(object sender, EventArgs e)
        //{
        //    await App.ViewModel.GetComic();
        //}

        private void FavoriteButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var selectedComic = button?.BindingContext as Comic;
            if (selectedComic == null) return;

            App.ViewModel.FavoriteComics.Add(selectedComic);

            MyRadListView.EndItemSwipe();
        }

        private void UnfavoriteButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var selectedComic = button?.BindingContext as Comic;
            if (selectedComic == null) return;

            if (App.ViewModel.FavoriteComics.Contains(selectedComic))
                App.ViewModel.FavoriteComics.Remove(selectedComic);

            MyRadListView.EndItemSwipe();
        }

        //private void NavigateToFavsButton_OnClicked(object sender, EventArgs e)
        //{
        //    App.RootPage.Navigation.PushAsync(new FavoritesPage());
        //}

        private async void MyRadListView_OnRefreshRequested(object sender, PullToRefreshRequestedEventArgs e)
        {
            await App.ViewModel.GetComic();
            MyRadListView.EndRefresh();
        }

        //moved to command
        //private void MyRadListView_OnItemTapped(object sender, ItemTapEventArgs e)
        //{
        //    var selectedComic = e.Item as Comic;
        //    if (selectedComic == null) return;

        //       var detailsPage = new DetailsPage();
        //    detailsPage.Title = selectedComic.Title;
        //    detailsPage.Icon = "ic_xkcd_light.png";

        //       var dpvm = detailsPage.BindingContext as DetailsPageViewModel;

        //       if (dpvm != null) dpvm.SelectedComic = selectedComic;

        //       App.RootPage.Navigation.PushAsync(detailsPage);
        //}
    }
}

