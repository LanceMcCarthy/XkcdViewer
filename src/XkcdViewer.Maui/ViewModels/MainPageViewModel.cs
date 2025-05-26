// ReSharper disable AsyncVoidLambda
using System.Collections.ObjectModel;
using System.Diagnostics;
using XkcdViewer.Common.Models;
using XkcdViewer.Common.Services;

namespace XkcdViewer.Maui.ViewModels;

public partial class MainPageViewModel : PageViewModelBase
{
    private readonly ComicDataService comicDataService;
    private Comic? currentComic;
    private bool getNewComicButtonIsVisible;
    private bool areCopilotControlsVisible;

    public MainPageViewModel(ComicDataService comicDataServ)
    {
        comicDataService = comicDataServ;

        FetchComicCommand = new Command(async (c) => await FetchComicAsync());
        ShareCommand = new Command(async c => await ShareItemAsync());
        ToggleFavoriteCommand = new Command(async (c) => await ToggleFavorite(CurrentComic));

#if WINDOWS
        DeleteComicCommand = new Command<int>(DeleteCachedComicImage);
        AnalyzeComicCommand = new Command(async () => await AnalyzeCurrentComicAsync());

        InitializeCopilotCapabilities();
#endif
    }

    public bool AreCopilotControlsVisible
    {
        get => areCopilotControlsVisible;
        set => SetProperty(ref areCopilotControlsVisible, value);
    }

    public ObservableCollection<Comic> Comics { get; } = [];

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

    public Command ShareCommand { get; set; }

    public Command ToggleFavoriteCommand { get; set; }

    public Command<int> DeleteComicCommand { get; set; }

    public Command AnalyzeComicCommand { get; set; }

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

    public override async void OnAppearing()
    {
        base.OnAppearing();

        // service handles empty or full list situations
        await comicDataService.LoadComicsAsync(Comics);
    }
}