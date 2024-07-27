using CommonHelpers.Common;
using System.Collections.ObjectModel;
using XkcdViewer.Maui.Models;
using XkcdViewer.Maui.Services;

namespace XkcdViewer.Maui.ViewModels;

public class FavoritesPageViewModel : ViewModelBase
{
    private readonly FavoritesService favoritesService;

    public FavoritesPageViewModel(FavoritesService favoritesSrv)
    {
        Title = "Favorites";
        favoritesService = favoritesSrv;
        FavoriteComics = favoritesSrv.Favorites;

        ToggleFavoriteCommand = new Command<Comic>(ToggleIsFavorite);
        ShareCommand = new Command<Comic>(async (c) => { await ShareItem(c); });
    }
        
    public ObservableCollection<Comic?> FavoriteComics { get; }

    public Command<Comic> ToggleFavoriteCommand { get; }

    public Command<Comic> ShareCommand { get; }

    private void ToggleIsFavorite(Comic? comic)
    {
        if (comic.IsFavorite)
        {
            favoritesService.RemoveFavorite(comic);
        }
        else
        {
            favoritesService.AddFavorite(comic);
        }
    }

    public async Task ShareItem(Comic comic)
    {
        if (string.IsNullOrEmpty(comic.Img))
            return;
            
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = Title ?? "xkcd",
            Text = comic.Transcript ?? "",
            Uri = comic.Img
        });
    }
}