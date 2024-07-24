using CommonHelpers.Common;
using CommonHelpers.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using XkcdViewer.Maui.Models;
using XkcdViewer.Maui.Services;

namespace XkcdViewer.Maui.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly XkcdApiService apiService;
    private readonly FavoritesService favoritesService;
    private int lastComicNumber;

    public MainViewModel(XkcdApiService apiServ, FavoritesService favoritesSrv)
    {
        Title = DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst
            ? "XKCD Comic Viewer"
            : "XKCD Viewer";

        favoritesService = favoritesSrv;
        apiService = apiServ;

        GoToDetailsCommand = new Command<Comic>(async (c) => await GoToDetailsAsync(c));
        ShareCommand = new Command<Comic>(async (c) => { await ShareItem(c); });
    }

    public ObservableCollection<Comic> Comics { get; } = new();

    public Command<Comic> GoToDetailsCommand { get; set; }

    public Command<Comic> ShareCommand { get; }

    public async Task GetNextComic()
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

    private static async Task GoToDetailsAsync(Comic comic)
    {
        await Shell.Current.GoToAsync("/AccountDetails", new Dictionary<string, object>
        {
            { "SelectedComic", comic }
        });
    }

    public async Task ShareItem(Comic? comic)
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
