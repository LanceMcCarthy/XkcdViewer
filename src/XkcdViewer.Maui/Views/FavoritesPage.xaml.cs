using Telerik.Maui.Controls.Compatibility.DataControls.ListView;
using XkcdViewer.Maui.Common;
using XkcdViewer.Maui.Models;

namespace XkcdViewer.Maui.Views;

public partial class FavoritesPage : ContentPage
{
    public FavoritesPage()
    {
        InitializeComponent();
    }

    private void Rlv_ReorderEnded(object sender, ReorderEndedEventArgs e)
    {
        FavoritesManager.Current.SaveFavorites();
    }

    private void Lv_ItemTapped(object sender, ItemTapEventArgs e)
    {
        Navigation.PushAsync(new DetailsPage(e.Item as Comic));
    }
}