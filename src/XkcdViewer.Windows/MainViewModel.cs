using CommonHelpers.Common;
using CommonHelpers.Mvvm;
using CommonHelpers.Services;
using Microsoft.Graphics.Imaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AI.ContentModeration;
using Microsoft.Windows.AI.Generative;
using Microsoft.Windows.Management.Deployment;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Core;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using XkcdViewer.Common.Models;
using XkcdViewer.Common.Services;
using XkcdViewer.Windows.Utils;

namespace XkcdViewer.Windows;

public class MainViewModel : ViewModelBase
{
    private static readonly bool IsPackagedApp = (Environment.GetEnvironmentVariable("PACKAGED_PRODUCT_ID") != null);
    private static HttpClient? httpClient;
    private static MediaPlayerElement? mpe;

    private readonly ComicDataService comicDataService;

    public MainViewModel()
    {
        comicDataService = new ComicDataService(new XkcdApiService());
        FetchComicCommand = new RelayCommand(async () => await FetchComicAsync());
        AnalyzeComicCommand = new RelayCommand(async () => await AnalyzeComicAsync());

        HasNpuCapability = AppUtils.HasNpu() ? Visibility.Visible : Visibility.Collapsed;
    }

    public ObservableCollection<Comic> Comics { get; } = [];

    public Comic? CurrentComic
    {
        get;
        set => SetProperty(ref field, value);
    }

    public RelayCommand FetchComicCommand { get; set; }

    public RelayCommand AnalyzeComicCommand { get; set; }

    public Visibility HasNpuCapability
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

    public async Task AnalyzeComicAsync()
    {
        if (IsBusy || CurrentComic?.Img is null)
            return;

        try
        {
            IsBusy = true;
            IsBusyMessage = "Analyzing...";

            const string fileName = "analyze.png";
            var filePath = IsPackagedApp ? "ms-appdata:///local/analyze.png" : Path.Combine(AppContext.BaseDirectory, fileName);

            // STEP 1 - Download image and save to temp working file
            IsBusyMessage = "Downloading image...";
            await DownloadImageAsync(filePath, CurrentComic.Img);

            // STEP 2 - Load png file into a SoftwareBitmap
            IsBusyMessage = "Analyzing image...";
            var languageModelResponse = await GetImageDescriptionAsync(filePath);

            // STEP 3 - Audio playback using SpeechSynthesizer.
            IsBusyMessage = "Playing back audio...";
            await ReadAloudAsync(languageModelResponse.Response);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        finally
        {
            IsBusyMessage = "";
            IsBusy = false;
        }
    }

    private static async Task DownloadImageAsync(string filePath, string imageUrl)
    {
        httpClient ??= new HttpClient();

        var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

        await File.WriteAllBytesAsync(filePath, imageBytes);
    }

    private async Task<LanguageModelResponse?> GetImageDescriptionAsync(string filePath)
    {
        try
        {
            IRandomAccessStream stream;

            if (IsPackagedApp)
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(filePath));
                stream = await file.OpenAsync(FileAccessMode.Read);
            }
            else
            {
                stream = File.OpenRead(filePath).AsRandomAccessStream();
            }

            if (stream == null)
            {
                await new MessageDialog("There was a problem loading the image file.").ShowAsync();
                return null;
            }

            // STEP 1 - Load png file into a SoftwareBitmap

            var decoder = await BitmapDecoder.CreateAsync(stream);
            var sBitmap = await decoder.GetSoftwareBitmapAsync();

            if (sBitmap == null)
            {
                await new MessageDialog("There was a problem creating the SoftwareBitmap from the image file.").ShowAsync();
                return null;
            }

            // STEP 2 - Make sure Gen AI capabilities are onboard

            IsBusyMessage = "Checking Windows AI capabilities...";

            if (!ImageDescriptionGenerator.IsAvailable())
            {
                IsBusyMessage = "Preparing ImageDescriptionGenerator for first time use...";

                var result = await ImageDescriptionGenerator.MakeAvailableAsync();

                if (result.Status != PackageDeploymentStatus.CompletedSuccess)
                {
                    await new MessageDialog($"There was a problem installing the required packages: {result.ExtendedError.Message}").ShowAsync();
                    return null;
                }
            }

            // STEP 3 - Load the SoftwareBitmap into an ImageBuffer

            IsBusyMessage = "Preparing image buffer...";

            var inputImage = ImageBuffer.CreateCopyFromBitmap(sBitmap);

            if (inputImage == null)
            {
                await new MessageDialog("There was a problem creating the ImageBuffer from the SoftwareBitmap.").ShowAsync();
                return null;
            }

            // STEP 4 - Use ImageDescriptionGenerator from Windows Gen AI APIs

            IsBusyMessage = "Analyzing image...";

            var imageDescriptionGenerator = await ImageDescriptionGenerator.CreateAsync();

            var languageModelResponse = await imageDescriptionGenerator.DescribeAsync(
                inputImage,
                ImageDescriptionScenario.DetailedNarration,
                new ContentFilterOptions
                {
                    PromptMinSeverityLevelToBlock = { ViolentContentSeverity = SeverityLevel.Medium },
                    ResponseMinSeverityLevelToBlock = { ViolentContentSeverity = SeverityLevel.Medium }
                });

            return languageModelResponse;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            throw;
        }
    }

    private static async Task ReadAloudAsync(string message)
    {
        var synthesizer = new SpeechSynthesizer();
        var synthesisStream = await synthesizer.SynthesizeTextToStreamAsync(message);

        mpe ??= new MediaPlayerElement();
        var mediaSource = MediaSource.CreateFromStream(synthesisStream, synthesisStream.ContentType);
        mpe.Source = mediaSource;
        mpe.MediaPlayer.Play();
    }
}