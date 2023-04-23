using CommonHelpers.Common;
using System.Collections.ObjectModel;
using System.Diagnostics;
using XkcdViewer.Maui.Common;
using XkcdViewer.Maui.Models;

namespace XkcdViewer.Maui.ViewModels;

public class MainViewModel : ViewModelBase
{
    private int lastComicNumber;

    public MainViewModel()
    {
        if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
        {
            Title = "XKCD Comic Viewer";
        }
        else
        {
            Title = "XKCD Viewer";
        } 
    }

    public ObservableCollection<Comic> Comics { get; } = new();

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
                var result = await App.ApiService.GetNewestComicAsync();
                comic = result.ToComic();
            }
            else
            {
                var result = await App.ApiService.GetComicAsync(lastComicNumber - 1);
                comic = result.ToComic();
            }

            if (comic == null)
            {
                throw new NullReferenceException($"Attempt to fetch comic #{lastComicNumber} failed.");
            }

            lastComicNumber = comic.Num;

            Comics.Add(comic);

            comic.IsFavorite = FavoritesManager.Current.IsFavorite(comic);
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
}
