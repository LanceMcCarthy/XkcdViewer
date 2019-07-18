using Xamarin.Forms;
using XkcdViewer.Forms.Common;
using XkcdViewer.Forms.Models;
using XkcdViewer.Forms.ViewModels;

namespace XkcdViewer.Forms.Views
{
    public partial class FavoritesPage : ContentPage
    {
        private readonly FavoritesPageViewModel vm;

        public FavoritesPage()
        {
            InitializeComponent();
            vm = new FavoritesPageViewModel();
            BindingContext = vm;
        }

        private void Rlv_ReorderEnded(object sender, Telerik.XamarinForms.DataControls.ListView.ReorderEndedEventArgs e)
        {
            FavoritesManager.Current.SaveFavorites();
        }

        private void Lv_ItemTapped(object sender, Telerik.XamarinForms.DataControls.ListView.ItemTapEventArgs e)
        {
            Navigation.PushAsync(new DetailsPage(e.Item as Comic));
        }
    }
}
