using CommonHelpers.Common;
using System.Collections.ObjectModel;
using XkcdViewer.Maui.Models;
using XkcdViewer.Maui.Services;

namespace XkcdViewer.Maui.ViewModels;

public class FavoritesPageViewModel : ViewModelBase
{
    private readonly FavoritesService favoritesService;
    private Comic currentFavorite;

    public FavoritesPageViewModel(FavoritesService favoritesSrv)
    {
        Title = "Favorites";
        favoritesService = favoritesSrv;

        ToggleFavoriteCommand = new Command(ToggleFavorite);
        ShareCommand = new Command(async c => await ShareItem());
    }

    public ObservableCollection<Comic?> FavoriteComics => favoritesService.Favorites;

    public Comic CurrentFavorite
    {
        get => currentFavorite;
        set => SetProperty(ref currentFavorite, value);
    }

    public Command ShareCommand { get; set; }

    public Command ToggleFavoriteCommand { get; set; }

    private void ToggleFavorite()
    {
        if (CurrentFavorite is { IsFavorite: true })
        {
            favoritesService.RemoveFavorite(CurrentFavorite);
        }
        else
        {
            favoritesService.AddFavorite(CurrentFavorite);
        }
    }

    public async Task ShareItem()
    {
        if (string.IsNullOrEmpty(CurrentFavorite.Img))
            return;
            
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = Title ?? "xkcd",
            Text = CurrentFavorite.Transcript ?? "",
            Uri = CurrentFavorite.Img
        });
    }
}