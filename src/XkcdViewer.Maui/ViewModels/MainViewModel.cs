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

    public MainViewModel(XkcdApiService apiServ, FavoritesService favoritesSrv)
    {
        favoritesService = favoritesSrv;
        apiService = apiServ;

        Title = DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst
            ? "XKCD Comic Viewer"
            : "XKCD Viewer";

        ShowFavoritesCommand = new Command<ItemTapCommandContext>(e => Shell.Current.GoToAsync("/Favorites", new Dictionary<string, object>
        {
            { "SelectedComic", e.Item }
        }));

        ShowComicDetailsCommand = new Command<Comic>(e => Shell.Current.GoToAsync("/Details", new Dictionary<string, object>
        {
            { "SelectedComic", e }
        }));
        
        ShareCommand = new Command<Comic>(c => Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = Title ?? "xkcd",
            Text = c.Transcript ?? "",
            Uri = c.Img
        }));

        GetLastComicCommand = new Command(async (c) => await GetNextComic());
    }

    public ObservableCollection<Comic> Comics { get; } = new();

    public Command GetLastComicCommand { get; }

    public ICommand ShowFavoritesCommand { get; set; }

    public Command<Comic> ShowComicDetailsCommand { get; set; }

    public Command<Comic> ShareCommand { get; }

    public ICollectionViewPage? CollectionViewPage { get; set; } = null;

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

            ScrollToLast();
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

    private void ScrollToLast()
    {
        var item = Comics.LastOrDefault();
        CollectionViewPage?.ScrollIntoView(item, true);
    }
}
