using XkcdViewer.Maui.ViewModels;
using System.Diagnostics;

#if WINDOWS
using Microsoft.Graphics.Imaging;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AI.ContentModeration;
using Microsoft.Windows.AI.Generative;
using Microsoft.Windows.Management.Deployment;
using Windows.Graphics.Imaging;
using Windows.Media.Core;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
using Windows.Storage;
using Microsoft.UI.Dispatching;
using XkcdViewer.Maui.WinUI;
#endif

namespace XkcdViewer.Maui.Views;

public partial class MainPage : BasePage
{
    private MainPageViewModel vm;

    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = vm = viewModel;
    }

#if WINDOWS
    private static readonly bool IsPackagedApp = (Environment.GetEnvironmentVariable("PACKAGED_PRODUCT_ID") != null);
    private static HttpClient? httpClient;
    private static MediaPlayerElement? mpe;

    private void InitializeCopilotCapabilities()
    {
        if (!AppUtils.HasNpu())
            return;

        // Shows the panel with Copilot controls
        //CopilotCapVisibility = Visibility.Visible;
        DescriptionLevelsComboBox.IsVisible = true;

        // Populate the ComboBox with the available description levels
        var levelsList = Enum.GetValues<ImageDescriptionScenario>();
        //DescriptionLevels.AddRange(levelsList);
        DescriptionLevelsComboBox.ItemsSource = levelsList;

        // Preselect the one I think works best
        //PreferredDescriptionLevel = DescriptionLevels.FirstOrDefault(n => n == ImageDescriptionScenario.DetailedNarration);
        DescriptionLevelsComboBox.SelectedItem = levelsList.FirstOrDefault(n => n == ImageDescriptionScenario.DetailedNarration);
    }

    public async Task AnalyzeCurrentComicAsync()
    {
        if (vm.IsBusy || vm.CurrentComic is null || string.IsNullOrEmpty(vm.CurrentComic.Img))
            return;

        try
        {
            vm.IsBusy = true;
            vm.IsBusyMessage = "Analyzing...";

            // STEP 1 - Download image and save to temp working file
            vm.IsBusyMessage = "Downloading image...";
            await DownloadImageAsync(vm.CurrentComic.Num, vm.CurrentComic.Img);

            // STEP 2 - Load png file into a SoftwareBitmap
            vm.IsBusyMessage = "Analyzing image...";
            var languageModelResponse = await GetImageDescriptionAsync(vm.CurrentComic.Num);

            if (languageModelResponse == null)
                return;

            // STEP 3 - Audio playback using SpeechSynthesizer.
            vm.IsBusyMessage = "Playing back audio...";
            await ReadAloudAsync(languageModelResponse.Response);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        finally
        {
            vm.IsBusyMessage = "";
            vm.IsBusy = false;
        }
    }

    private static async Task DownloadImageAsync(int comicId, string url)
    {
        var filePath = IsPackagedApp
            ? Path.Combine(ApplicationData.Current.TemporaryFolder.Path, $"{comicId}.png")
            : Path.Combine(AppContext.BaseDirectory, $"{comicId}.png");

        if (File.Exists(filePath))
            File.Delete(filePath);

        httpClient ??= new HttpClient();

        var imageBytes = await httpClient.GetByteArrayAsync(url);

        await File.WriteAllBytesAsync(filePath, imageBytes);
    }

    private static void DeleteCachedComicImage(int comicId)
    {
        var filePath = IsPackagedApp
            ? Path.Combine(ApplicationData.Current.TemporaryFolder.Path, $"{comicId}.png")
            : Path.Combine(AppContext.BaseDirectory, $"{comicId}.png");

        if (!File.Exists(filePath))
            return;

        File.Delete(filePath);
    }

    private async Task<LanguageModelResponse?> GetImageDescriptionAsync(int comicId)
    {
        var filePath = IsPackagedApp
            ? Path.Combine(ApplicationData.Current.TemporaryFolder.Path, $"{comicId}.png")
            : Path.Combine(AppContext.BaseDirectory, $"{comicId}.png");

        if (!File.Exists(filePath))
        {
            await ShowAnalyzerMessageAsync("Local image file does not exist, cannot proceed.");
            return null;
        }

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

        if (stream is null)
        {
            await ShowAnalyzerMessageAsync("There was a problem loading the image file.");
            return null;
        }

        // STEP 1 - Load png file into a SoftwareBitmap

        var decoder = await BitmapDecoder.CreateAsync(stream);
        var sBitmap = await decoder.GetSoftwareBitmapAsync();

        if (sBitmap is null)
        {
            await ShowAnalyzerMessageAsync("There was a problem creating the SoftwareBitmap from the image file.");
            return null;
        }

        // STEP 2 - Make sure Gen AI capabilities are onboard

        vm.IsBusyMessage = "Checking Windows AI capabilities...";

        if (!ImageDescriptionGenerator.IsAvailable())
        {
            vm.IsBusyMessage = "Preparing ImageDescriptionGenerator for first time use...";

            var wProg = ImageDescriptionGenerator.MakeAvailableAsync();

            wProg.Progress = (result, progressInfo) =>
            {
                DispatcherQueue.GetForCurrentThread().TryEnqueue(() => vm.IsBusyMessage = $"Downloading model... {progressInfo.Progress * 100}% complete.");
            };

            PackageDeploymentResult? result = await wProg;

            if (result.Status != PackageDeploymentStatus.CompletedSuccess)
            {
                await ShowAnalyzerMessageAsync($"There was a problem installing the required packages: {result.ExtendedError.Message}");
                return null;
            }
        }

        // STEP 3 - Load the SoftwareBitmap into an ImageBuffer

        vm.IsBusyMessage = "Preparing image buffer...";

        var inputImage = ImageBuffer.CreateCopyFromBitmap(sBitmap);

        if (inputImage is null)
        {
            await ShowAnalyzerMessageAsync("There was a problem creating the ImageBuffer from the SoftwareBitmap.");
            return null;
        }

        // STEP 4 - Use ImageDescriptionGenerator from Windows Gen AI APIs

        vm.IsBusyMessage = "Requesting an ImageDescriptionGenerator for use...";

        var imageDescriptionGenerator = await ImageDescriptionGenerator.CreateAsync();



        vm.IsBusyMessage = "Analyzing image...";

        //var languageModelResponse = await imageDescriptionGenerator.DescribeAsync(
        //    inputImage,
        //    ImageDescriptionScenario.Accessibility,
        //    new ContentFilterOptions
        //    {
        //        PromptMinSeverityLevelToBlock = { ViolentContentSeverity = SeverityLevel.Medium },
        //        ResponseMinSeverityLevelToBlock = { ViolentContentSeverity = SeverityLevel.Medium }
        //    });

        var describer = imageDescriptionGenerator.DescribeAsync(
            inputImage,
            ImageDescriptionScenario.Accessibility,
            new ContentFilterOptions
            {
                PromptMinSeverityLevelToBlock = { ViolentContentSeverity = SeverityLevel.Medium },
                ResponseMinSeverityLevelToBlock = { ViolentContentSeverity = SeverityLevel.Medium }
            });

        describer.Progress = (response, progress) =>
        {
            DispatcherQueue.GetForCurrentThread().TryEnqueue(() => vm.IsBusyMessage = $"Preparing response... {progress}% complete.");
        };

        var languageModelResponse = await describer;

        return languageModelResponse;
    }

    private async Task ReadAloudAsync(string message)
    {
        var synthesizer = new SpeechSynthesizer();
        var synthesisStream = await synthesizer.SynthesizeTextToStreamAsync(message);

        if (synthesisStream is null)
        {
            await ShowAnalyzerMessageAsync("Could not generate synthesized audio.");
            return;
        }

        mpe ??= new MediaPlayerElement();
        var mediaSource = MediaSource.CreateFromStream(synthesisStream, synthesisStream.ContentType);
        mpe.Source = mediaSource;

        vm.IsBusyMessage = "Playing audio...";

        mpe.MediaPlayer.Play();

        vm.IsBusyMessage = "Done!";
    }

    private async Task ShowAnalyzerMessageAsync(string message)
    {
        var dialog = new ContentDialog
        {
            Title = "Alert",
            Content = message,
            PrimaryButtonText = "OK",
            DefaultButton = ContentDialogButton.Primary
        };

        await Shell.Current.DisplayAlert("Alert", message, "ok");
    }
#endif
}