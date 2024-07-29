using CommonHelpers.Common;
using CommonHelpers.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Telerik.Maui.Controls.Compatibility.DataControls.ListView.Commands;
using XkcdViewer.Maui.Models;
using XkcdViewer.Maui.Services;

namespace XkcdViewer.Maui.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly XkcdApiService apiService;
    private readonly FavoritesService favoritesService;
    private int lastComicNumber;
    private Comic currentComic;

    public MainViewModel(XkcdApiService apiServ, FavoritesService favoritesSrv)
    {
        favoritesService = favoritesSrv;
        apiService = apiServ;

        Title = DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst
            ? "XKCD Comic Viewer"
            : "XKCD Viewer";

        ShowFavoritesCommand = new Command<ItemTapCommandContext>(e => Shell.Current.GoToAsync("/Favorites", new Dictionary<string, object>{{ "SelectedComic", e.Item }}));

        ShareCommand = new Command(async c => await ShareItem());
        ShowComicDetailsCommand = new Command(async e => await ShowFavorites());
        FetchComicCommand = new Command(async (c) => await FetchComic());

        ToggleFavoriteCommand = new Command(ToggleFavorite);
    }

    public ObservableCollection<Comic> Comics { get; } = new();

    public Comic CurrentComic
    {
        get => currentComic;
        set => SetProperty(ref currentComic, value);
    }

    public Command FetchComicCommand { get; set; }

    public ICommand ShowFavoritesCommand { get; set; }

    public Command ShowComicDetailsCommand { get; set; }

    public Command ShareCommand { get; set; }

    public Command ToggleFavoriteCommand { get; set; }

    public ICollectionViewPage? CollectionViewPage { get; set; } = null;

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
        if (string.IsNullOrEmpty(CurrentComic.Img))
            return;
            
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = Title ?? "xkcd",
            Text = CurrentComic.Transcript ?? "",
            Uri = CurrentComic.Img
        });
    }

    public async Task ShowFavorites()
    {
        await Shell.Current.GoToAsync("/Details", new Dictionary<string, object>
        {
            { "SelectedComic", this.CurrentComic }
        });
    }

    private void ScrollToLast()
    {
        var item = Comics.LastOrDefault();
        CollectionViewPage?.ScrollIntoView(item, true);
    }
}
