using Telerik.Maui.Controls.Compatibility.DataControls.ListView;
using XkcdViewer.Maui.Models;
using XkcdViewer.Maui.Services;
using XkcdViewer.Maui.ViewModels;

namespace XkcdViewer.Maui.Views;

public partial class FavoritesPage : ContentPage
{
    private readonly FavoritesPageViewModel viewModel;
    private readonly FavoritesService favoritesService;

    public FavoritesPage(FavoritesPageViewModel vm, FavoritesService favoritesSrv)
    {
        InitializeComponent();
        favoritesService = favoritesSrv;
        viewModel = vm;
    }

    private void Rlv_ReorderEnded(object sender, ReorderEndedEventArgs e)
    {
        favoritesService.SaveFavorites();
    }

    private async void Lv_ItemTapped(object sender, ItemTapEventArgs e)
    {
        await Shell.Current.GoToAsync("/AccountDetails", new Dictionary<string, object>
        {
            {"SelectedComic", e.Item}
        });
    }
}