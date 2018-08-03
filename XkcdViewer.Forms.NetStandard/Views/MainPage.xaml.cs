using System;
using Xamarin.Forms;
using XkcdViewer.Forms.NetStandard.Models;
using XkcdViewer.Forms.NetStandard.ViewModels;

namespace XkcdViewer.Forms.NetStandard.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is MainViewModel viewModel && viewModel.Comics.Count == 0)
            {
                await viewModel.GetNextComic();
            }
        }

        private async void ListView_LoadOnDemand(object sender, EventArgs e)
        {
            if (BindingContext is MainViewModel viewModel)
            {
                await viewModel.GetNextComic();
            }
        }

        private void Lv_ItemTapped(object sender, Telerik.XamarinForms.DataControls.ListView.ItemTapEventArgs e)
        {
            Navigation.PushAsync(new DetailsPage(e.Item as Comic));
        }

        private void FavsPageToolbarItem_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new FavoritesPage());
        }
    }
}