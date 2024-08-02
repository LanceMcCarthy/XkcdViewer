using CommonHelpers.Common;
using System.Collections.ObjectModel;
using XkcdViewer.Maui.Models;
using XkcdViewer.Maui.Services;

namespace XkcdViewer.Maui.ViewModels;

public class FavoritesPageViewModel : PageViewModelBase
{
    private readonly ComicDataService comicDataService;
    private readonly MainPageViewModel mainViewModel;
    private Comic? currentFavorite;
    private ObservableCollection<Comic> favoriteComics;

    public FavoritesPageViewModel(ComicDataService comicDataServ, MainPageViewModel mainVm)
    {
        comicDataService = comicDataServ;
        mainViewModel = mainVm;
        Title = "Favorites";

        ShareCommand = new Command(async c => await ShareItem());

        ToggleFavoriteCommand = new Command<Comic>(async c =>
        {
            await ToggleFavorite(c);
        });
    }

    public ObservableCollection<Comic> FavoriteComics
    {
        get => favoriteComics;
        set => SetProperty(ref favoriteComics, value);
    }

    public Comic? CurrentFavorite
    {
        get => currentFavorite;
        set => SetProperty(ref currentFavorite, value);
    }

    public Command ShareCommand { get; set; }

    public Command<Comic> ToggleFavoriteCommand { get; set; }

    private async Task ToggleFavorite(Comic comic)
    {
        comic.IsFavorite = !comic.IsFavorite;

        await comicDataService.SaveComicsAsync(mainViewModel.Comics);
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

    public override void OnNavigatedTo(NavigatedToEventArgs args, ObservableCollection<Comic>? favorites)
    {
        base.OnNavigatedTo(args, favorites);

        this.FavoriteComics = favorites;
    }


}