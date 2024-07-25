using Telerik.Maui.Controls.Compatibility.DataControls.ListView;
using XkcdViewer.Maui.Services;
using XkcdViewer.Maui.ViewModels;

namespace XkcdViewer.Maui.Views;

public partial class FavoritesPage : ContentPage
{
    private readonly FavoritesService favoritesService;

    public FavoritesPage(FavoritesPageViewModel vm, FavoritesService favoritesSrv)
    {
        InitializeComponent();
        favoritesService = favoritesSrv;
        BindingContext = vm;
    }

    private void Rlv_ReorderEnded(object sender, ReorderEndedEventArgs e)
    {
        favoritesService.SaveFavorites();
    }

    private async void Lv_ItemTapped(object sender, ItemTapEventArgs e)
    {
        await Shell.Current.GoToAsync("/Details", new Dictionary<string, object>
        {
            { "SelectedComic", e.Item }
        });
    }
}