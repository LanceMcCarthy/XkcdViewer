using CommonHelpers.Common;
using CommonHelpers.Mvvm;
using CommonHelpers.Services;
using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using XkcdViewer.Common.Models;
using XkcdViewer.Common.Services;

namespace XkcdViewer.Windows;

public class MainViewModel : ViewModelBase
{
    private readonly ComicDataService comicDataService;
    private Comic? currentComic;

    public MainViewModel()
    {
        comicDataService = new ComicDataService(new XkcdApiService());
        FetchComicCommand = new RelayCommand(async () => await FetchComicAsync());
    }

    public ObservableCollection<Comic> Comics { get; } = [];

    public Comic? CurrentComic
    {
        get => currentComic;
        set => SetProperty(ref currentComic, value);
    }

    public RelayCommand FetchComicCommand { get; set; }

    public async Task FetchComicAsync()
    {
        try
        {
            if (IsBusy)
                return;

            IsBusy = true;
            IsBusyMessage = "Fetching Comic...";

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
            IsBusyMessage = "";
        }
    }

    public async Task InitialLoadAsync()
    {
        await comicDataService.LoadComicsAsync(Comics);
    }
}