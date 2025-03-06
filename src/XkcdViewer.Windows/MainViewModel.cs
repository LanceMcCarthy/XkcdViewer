using CommonHelpers.Common;
using CommonHelpers.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommonHelpers.Services;
using XkcdViewer.Common.Models;
using XkcdViewer.Common.Services;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Telerik.UI.Xaml.Controls.Input;

namespace XkcdViewer.Windows;

public class MainViewModel : ViewModelBase
{
    private readonly ComicDataService comicDataService;
    private Comic? currentComic;
    private Visibility getNewComicButtonIsVisible;
    private string selectedSegment = "Comics";

    public MainViewModel()
    {
        comicDataService = new ComicDataService(new XkcdApiService());

        FetchComicCommand = new RelayCommand(async () => await FetchComicAsync());
        ShareCommand = new RelayCommand(async () => await ShareItemAsync());
        ToggleFavoriteCommand = new RelayCommand(async ()=> await ToggleFavorite(CurrentComic));
    }

    public ObservableCollection<Comic> Comics { get; } = [];

    public Comic? CurrentComic
    {
        get => currentComic;
        set
        {
            if (SetProperty(ref currentComic, value))
            {
                GetNewComicButtonIsVisible = Comics.LastOrDefault() == currentComic ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }

    public Visibility GetNewComicButtonIsVisible
    {
        get => getNewComicButtonIsVisible;
        set => SetProperty(ref getNewComicButtonIsVisible, value);
    }

    public string SelectedSegment
    {
        get => selectedSegment;
        set => SetProperty(ref selectedSegment, value);
    }

    public RelayCommand FetchComicCommand { get; set; }

    public RelayCommand ShareCommand { get; set; }

    public RelayCommand ToggleFavoriteCommand { get; set; }

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

            await comicDataService.SaveComicsAsync(Comics);
        }
    }

    public async Task ShareItemAsync()
    {
        if (string.IsNullOrEmpty(currentComic?.Img))
            return;

        //await Share.Default.RequestAsync(new ShareTextRequest
        //{
        //    Title = Title ?? "xkcd",
        //    Text = currentComic.Transcript ?? "",
        //    Uri = currentComic.Img
        //});
    }

    public async Task InitialLoadAsync()
    {
        await comicDataService.LoadComicsAsync(Comics);
    }
}