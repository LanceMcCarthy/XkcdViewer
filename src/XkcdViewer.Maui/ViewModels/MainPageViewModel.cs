using CommonHelpers.Common;
using CommonHelpers.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using XkcdViewer.Maui.Models;
using XkcdViewer.Maui.Services;

namespace XkcdViewer.Maui.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    //private readonly XkcdApiService apiService;
    //private readonly FavoritesService favoritesService;
    private readonly DataService dataService;
    //private int lastComicNumber;
    private Comic? currentComic;
    private bool getNewComicButtonIsVisible;

    public MainPageViewModel(DataService dataServ, XkcdApiService apiServ, FavoritesService favoritesSrv)
    {
        //favoritesService = favoritesSrv;
        //apiService = apiServ;
        dataService = dataServ;

        Title = DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst
            ? "XKCD Comic Viewer"
            : "XKCD Viewer";

        ShowFavoritesCommand = new Command(async e => await ShowFavorites());
        ShareCommand = new Command(async c => await ShareItem());
        FetchComicCommand = new Command(async (c) => await FetchComic());

        ToggleFavoriteCommand = new Command(ToggleFavorite);
    }

    public ObservableCollection<Comic>? Comics => dataService.Comics;

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

    public async Task OnAppearing()
    {
        await dataService.LoadComicsAsync();
    }

    public async Task FetchComic()
    {
        try
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await dataService.LoadNewComic();

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

    private void ToggleFavorite()
    {
        if (CurrentComic != null)
        {
            Comics.FirstOrDefault(c => c.Num == CurrentComic.Num).IsFavorite = Comics.FirstOrDefault(c => c.Num == CurrentComic.Num).IsFavorite!;
        }
        
        //if (CurrentComic is { IsFavorite: true })
        //{
        //    favoritesService.RemoveFavorite(CurrentComic);
        //}
        //else
        //{
        //    favoritesService.AddFavorite(CurrentComic);
        //}
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
        var favorites = Comics.Where(c => c.IsFavorite);

        await Shell.Current.GoToAsync("/Favorites", new Dictionary<string, object>
        {
            { "SelectedComic", favorites }
        });
    }
}
