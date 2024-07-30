using CommonHelpers.Common;
using CommonHelpers.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Telerik.Maui.Controls.Compatibility.DataControls.ListView.Commands;
using XkcdViewer.Maui.Models;
using XkcdViewer.Maui.Services;

namespace XkcdViewer.Maui.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    private readonly XkcdApiService apiService;
    private readonly FavoritesService favoritesService;
    private int lastComicNumber;
    private Comic? currentComic;

    public MainPageViewModel(XkcdApiService apiServ, FavoritesService favoritesSrv)
    {
        favoritesService = favoritesSrv;
        apiService = apiServ;

        Title = DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst
            ? "XKCD Comic Viewer"
            : "XKCD Viewer";

        ShowFavoritesCommand = new Command<ItemTapCommandContext>(e => Shell.Current.GoToAsync("/Favorites", new Dictionary<string, object>{{ "SelectedComic", e.Item }}));

        ShareCommand = new Command(async c => await ShareItem());
        FetchComicCommand = new Command(async (c) => await FetchComic());

        ToggleFavoriteCommand = new Command(ToggleFavorite);
    }

    public ObservableCollection<Comic> Comics { get; } = new();

    public Comic? CurrentComic
    {
        get => currentComic;
        set => SetProperty(ref currentComic, value);
    }

    public Command FetchComicCommand { get; set; }

    public Command ShowFavoritesCommand { get; set; }

    public Command ShareCommand { get; set; }

    public Command ToggleFavoriteCommand { get; set; }

    public async Task FetchComic()
    {
        try
        {
            if (IsBusy)
                return;

            IsBusy = true;

            Comic comic;

            if (lastComicNumber == 0)
            {
                var result = await apiService.GetNewestComicAsync();
                comic = result.ToComic();
            }
            else
            {
                var result = await apiService.GetComicAsync(lastComicNumber - 1);
                comic = result.ToComic();
            }

            if (comic == null)
            {
                throw new NullReferenceException($"Attempt to fetch comic #{lastComicNumber} failed.");
            }

            lastComicNumber = comic.Num;

            Comics.Add(comic);

            comic.IsFavorite = favoritesService.IsFavorite(comic);

            CurrentComic = comic;
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
        if (CurrentComic is { IsFavorite: true })
        {
            favoritesService.RemoveFavorite(CurrentComic);
        }
        else
        {
            favoritesService.AddFavorite(CurrentComic);
        }
    }

    public async Task ShareItem()
    {
        if (string.IsNullOrEmpty(currentComic.Img))
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
        await Shell.Current.GoToAsync("/Details", new Dictionary<string, object>
        {
            { "SelectedComic", currentComic }
        });
    }
}
