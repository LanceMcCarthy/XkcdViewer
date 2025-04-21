using CommonHelpers.Collections;
using CommonHelpers.Common;
using CommonHelpers.Mvvm;
using CommonHelpers.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AI.Generative;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using XkcdViewer.Common.Models;
using XkcdViewer.Common.Services;
using XkcdViewer.Windows.Utils;

namespace XkcdViewer.Windows;

public partial class MainViewModel : ViewModelBase
{
    private readonly ComicDataService comicDataService;
    
    public MainViewModel()
    {
        comicDataService = new ComicDataService(new XkcdApiService());
        FetchComicCommand = new RelayCommand(async () => await FetchComicAsync());
        DeleteComicCommand = new RelayCommand(async () => await DeleteComicAsync());
        AnalyzeComicCommand = new RelayCommand(async () => await AnalyzeCurrentComicAsync());

        InitializeCopilotCapabilities();
    }

    public ObservableRangeCollection<Comic> Comics { get; } = [];

    public ObservableRangeCollection<ImageDescriptionScenario> DescriptionLevels { get; } = [];

    public Comic? CurrentComic
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageDescriptionScenario PreferredDescriptionLevel
    {
        get;
        set => SetProperty(ref field, value);
    }

    public RelayCommand FetchComicCommand { get; set; }

    public RelayCommand DeleteComicCommand { get; set; }

    public RelayCommand AnalyzeComicCommand { get; set; }

    public required IDialogService DialogService { get; set; }

    public Visibility CopilotCapVisibility
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task InitialLoadAsync()
    {
        await comicDataService.LoadComicsAsync(Comics);
    }

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

    private async Task DeleteComicAsync()
    {
        if (CurrentComic is null)
            return;

        var dialog = new ContentDialog
        {
            Title = "Delete Confirmation",
            Content = "Are you sure you want to remove this comic?",
            PrimaryButtonText = "YES",
            CloseButtonText = "NO",
            DefaultButton = ContentDialogButton.Close
        };

        var result = await this.DialogService.ShowDialogAsync(dialog);

        if (result != ContentDialogResult.Primary)
            return;

        var nextIndex = Comics.IndexOf(CurrentComic) - 1;
        if(nextIndex <= 0)
            nextIndex = 0;

        DeleteCachedComicImage(CurrentComic.Num);

        Comics.Remove(CurrentComic);

        CurrentComic = Comics.ElementAtOrDefault(nextIndex);

        await comicDataService.SaveComicsAsync(Comics);
    }
}