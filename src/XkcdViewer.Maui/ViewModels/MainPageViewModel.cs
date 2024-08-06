// ReSharper disable AsyncVoidLambda
using System.Collections.ObjectModel;
using System.Diagnostics;
using XkcdViewer.Maui.Models;
using XkcdViewer.Maui.Services;

namespace XkcdViewer.Maui.ViewModels;

public class MainPageViewModel : PageViewModelBase
{
    private readonly ComicDataService comicDataService;
    private Comic? currentComic;
    private bool getNewComicButtonIsVisible;

    public MainPageViewModel(ComicDataService comicDataServ)
    {
        comicDataService = comicDataServ;

        Title = DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst ? "XKCD Comic Viewer" : "XKCD Viewer";

        FetchComicCommand = new Command(async (c) => await FetchComicAsync());
        ShareCommand = new Command(async c => await ShareItemAsync());
        ShowFavoritesCommand = new Command(async e => await ShowFavoritesAsync());
        ToggleFavoriteCommand = new Command(async (c) => await ToggleFavorite(CurrentComic));
    }

    public ObservableCollection<Comic> Comics { get; } = new();

    public Comic? CurrentComic
    {
        get => currentComic;
        set
        {
            if (SetProperty(ref currentComic, value))
            {
                GetNewComicButtonIsVisible = Comics.LastOrDefault() == currentComic;
            }
        }
    }

    public bool GetNewComicButtonIsVisible
    {
        get => getNewComicButtonIsVisible;
        set => SetProperty(ref getNewComicButtonIsVisible, value);
    }

    public Command FetchComicCommand { get; set; }

    public Command ShowFavoritesCommand { get; set; }

    public Command ShareCommand { get; set; }

    public Command ToggleFavoriteCommand { get; set; }

    public async Task FetchComicAsync()
    {
        try
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await comicDataService.GetComic(Comics, Comics.LastOrDefault()?.Num - 1);

            CurrentComic = Comics.Last();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"FetchComicAsync Exception\r\n{ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ToggleFavorite(Comic? comic)
    {
        if (comic != null)
        {
            comic.IsFavorite = !comic.IsFavorite;

            await comicDataService.SaveComicsAsync(this.Comics);
        }
    }

    public async Task ShareItemAsync()
    {
        if (string.IsNullOrEmpty(currentComic?.Img))
            return;
            
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = Title ?? "xkcd",
            Text = currentComic.Transcript ?? "",
            Uri = currentComic.Img
        });
    }

    public async Task ShowFavoritesAsync()
    {
        var favorites = Comics.Where(c => c.IsFavorite);

        await Shell.Current.GoToAsync("/Favorites", new Dictionary<string, object>
        {
            { "SelectedComic", favorites }
        });
    }

    public override async void OnAppearing()
    {
        base.OnAppearing();

        await comicDataService.LoadComicsAsync(Comics);
    }
}