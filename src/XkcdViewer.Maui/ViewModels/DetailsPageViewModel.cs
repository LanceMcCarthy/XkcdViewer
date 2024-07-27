using CommonHelpers.Common;
using XkcdViewer.Maui.Models;
using XkcdViewer.Maui.Services;

namespace XkcdViewer.Maui.ViewModels;

public class DetailsPageViewModel : ViewModelBase
{
    private Comic? selectedComic;
    private readonly FavoritesService favoritesService;

    public DetailsPageViewModel(FavoritesService favoritesSrv)
    {
        favoritesService = favoritesSrv;
        
        ToggleFavoriteCommand = new Command(ToggleIsFavorite);
        ShareCommand = new Command(async (c) => { await ShareItem(); });
    }

    public Comic? SelectedComic
    {
        get => selectedComic;
        set
        {
            if (SetProperty(ref selectedComic, value))
            {
                this.Title = $"#{SelectedComic?.Num}";
            }
        }
    }

    public Command ToggleFavoriteCommand { get; }

    public Command ShareCommand { get; }

    private void ToggleIsFavorite()
    {
        if (SelectedComic is { IsFavorite: true })
        {
            favoritesService.RemoveFavorite(SelectedComic);
        }
        else
        {
            favoritesService.AddFavorite(SelectedComic);
        }
    }

    public async Task ShareItem()
    {
        if (string.IsNullOrEmpty(SelectedComic?.Img))
            return;
            
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = Title ?? "xkcd",
            Text = SelectedComic.Transcript ?? "",
            Uri = SelectedComic.Img
        });
    }
}