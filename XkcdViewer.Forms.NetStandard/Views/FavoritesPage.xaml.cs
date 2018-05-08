using Xamarin.Forms;
using XkcdViewer.Forms.NetStandard.Common;
using XkcdViewer.Forms.NetStandard.Models;
using XkcdViewer.Forms.NetStandard.ViewModels;

namespace XkcdViewer.Forms.NetStandard.Views
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
