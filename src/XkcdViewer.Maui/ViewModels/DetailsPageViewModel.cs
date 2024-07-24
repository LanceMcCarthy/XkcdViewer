using CommonHelpers.Common;
using XkcdViewer.Maui.Models;
using XkcdViewer.Maui.Services;

namespace XkcdViewer.Maui.ViewModels;

public class DetailsPageViewModel : ViewModelBase
{
    private Comic selectedComic;
    private readonly FavoritesService favoritesService;

    public DetailsPageViewModel(FavoritesService favoritesSrv)
    {
        favoritesService = favoritesSrv;
        
        ToggleFavoriteCommand = new Command(ToggleIsFavorite);
    }

    public Comic SelectedComic
    {
        get => selectedComic;
        set
        {
            if (SetProperty(ref selectedComic, value))
            {
                this.Title = $"#{SelectedComic.Num}";
            }
        }
    }

    public Command ToggleFavoriteCommand { get; }

    private void ToggleIsFavorite()
    {
        if (SelectedComic.IsFavorite)
        {
            favoritesService.RemoveFavorite(SelectedComic);
        }
        else
        {
            favoritesService.AddFavorite(SelectedComic);
        }
    }
}