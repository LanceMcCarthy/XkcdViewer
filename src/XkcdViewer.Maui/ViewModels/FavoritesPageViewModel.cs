// ReSharper disable AsyncVoidLambda
using System.Collections.ObjectModel;
using XkcdViewer.Maui.Models;
using XkcdViewer.Maui.Services;

namespace XkcdViewer.Maui.ViewModels;

public class FavoritesPageViewModel : PageViewModelBase
{
    private readonly ComicDataService comicDataService;
    private readonly MainPageViewModel mainViewModel;
    private Comic? currentFavorite;

    public FavoritesPageViewModel(ComicDataService comicDataServ, MainPageViewModel mainVm)
    {
        comicDataService = comicDataServ;
        mainViewModel = mainVm;
        Title = "Favorites";

        ShareCommand = new Command(async c => await ShareItem());

        ToggleFavoriteCommand = new Command(async c =>{await ToggleFavorite();});
    }

    public ObservableCollection<Comic> FavoriteComics { get; } = [];

    public Comic? CurrentFavorite
    {
        get => currentFavorite;
        set => SetProperty(ref currentFavorite, value);
    }

    public Command ShareCommand { get; set; }

    public Command ToggleFavoriteCommand { get; set; }

    private async Task ToggleFavorite()
    {
        if (CurrentFavorite == null)
            return;

        var originalFavRef = mainViewModel.Comics.FirstOrDefault(c => c.Num == CurrentFavorite?.Num);

        if (originalFavRef != null)
        {
            originalFavRef.IsFavorite = false;
            await comicDataService.SaveComicsAsync(mainViewModel.Comics);
        }

        FavoriteComics.Remove(CurrentFavorite);
    }

    public async Task ShareItem()
    {
        if (string.IsNullOrEmpty(CurrentFavorite?.Img))
            return;
            
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = Title ?? "xkcd",
            Text = CurrentFavorite.Transcript ?? "",
            Uri = CurrentFavorite.Img
        });
    }

    public override async void OnAppearing()
    {
        base.OnAppearing();

        // service handles empty or full list situations
        await comicDataService.LoadFavoriteComicsAsync(FavoriteComics);
    }
}