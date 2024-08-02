using System.Collections.ObjectModel;
using System.Diagnostics;
using XkcdViewer.Maui.Models;
using XkcdViewer.Maui.Services;

namespace XkcdViewer.Maui.ViewModels;

public class MainPageViewModel : PageViewModelBase
{
    //private readonly XkcdApiService apiService;
    //private readonly FavoritesService favoritesService;
    private readonly ComicDataService comicDataService;
    //private int lastComicNumber;
    private Comic? currentComic;
    private bool getNewComicButtonIsVisible;

    public MainPageViewModel(ComicDataService comicDataServ)
    {
        //favoritesService = favoritesSrv;
        //apiService = apiServ;
        comicDataService = comicDataServ;

        Title = DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst ? "XKCD Comic Viewer" : "XKCD Viewer";

        FetchComicCommand = new Command(async (c) => await FetchComic());
        ShareCommand = new Command(async c => await ShareItem());
        ShowFavoritesCommand = new Command(async e => await ShowFavorites());
        ToggleFavoriteCommand = new Command(async (c) => await ToggleFavorite(CurrentComic));
    }

    public ObservableCollection<Comic>? Comics { get; } = new();

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

    public async Task FetchComic()
    {
        try
        {
            if (IsBusy || Comics == null)
                return;

            IsBusy = true;

            await comicDataService.GetComic(Comics, Comics.LastOrDefault()?.Num - 1);

            //Comic comic;

            //if (lastComicNumber == 0)
            //{
            //    var result = await apiService.GetNewestComicAsync();
            //    comic = result.ToComic();
            //}
            //else
            //{
            //    var result = await apiService.GetComicAsync(lastComicNumber - 1);
            //    comic = result.ToComic();
            //}

            //if (comic == null)
            //{
            //    throw new NullReferenceException($"Attempt to fetch comic #{lastComicNumber} failed.");
            //}

            //lastComicNumber = comic.Num;

            //Comics.Add(comic);

            //comic.IsFavorite = favoritesService.IsFavorite(comic);

            CurrentComic = Comics.Last();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"LoadComicsAsync Exception\r\n{ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ToggleFavorite(Comic comic)
    {
        if (Comics == null)
            return;

        comic.IsFavorite = !comic.IsFavorite;

        await comicDataService.SaveComicsAsync(this.Comics);
    }

    public async Task ShareItem()
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

    public async Task ShowFavorites()
    {
        if (Comics == null)
            return;

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