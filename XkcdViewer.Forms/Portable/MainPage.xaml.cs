using System;
using Portable.Models;
using Telerik.XamarinForms.DataControls.ListView;
using Xamarin.Forms;

namespace Portable
{
	public partial class MainPage : ContentPage
	{
		public MainPage ()
		{
			InitializeComponent();
		    //this.BindingContext = App.ViewModel; //set in XAML now
        }

	    private async void GetNextComicButton_OnClicked(object sender, EventArgs e)
	    {
            await App.ViewModel.GetComic();
        }
        
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

        private void NavigateToFavsButton_OnClicked(object sender, EventArgs e)
	    {
	        Navigation.PushAsync(new FavoritesPage());
	    }

	    private async void MyRadListView_OnRefreshRequested(object sender, PullToRefreshRequestedEventArgs e)
	    {
            await App.ViewModel.GetComic();
            MyRadListView.EndRefresh();
        }

	    private void MyRadListView_OnItemTapped(object sender, ItemTapEventArgs e)
	    {
	        throw new NotImplementedException();
	    }
	}
}

