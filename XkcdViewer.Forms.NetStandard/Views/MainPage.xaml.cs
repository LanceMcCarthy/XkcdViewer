using Xamarin.Forms;
using XkcdViewer.Forms.NetStandard.Models;
using XkcdViewer.Forms.NetStandard.ViewModels;

namespace XkcdViewer.Forms.NetStandard.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel vm;

        public MainPage()
        {
            InitializeComponent();
            vm = new MainViewModel();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await vm.GetNextComic();
        }

        private async void GetNextToolbarItem_Clicked(object sender, System.EventArgs e)
        {
            await vm.GetNextComic();
        }

        private async void Lv_RefreshRequested(object sender, Telerik.XamarinForms.DataControls.ListView.PullToRefreshRequestedEventArgs e)
        {
            await vm.GetNextComic();
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