// ReSharper disable AsyncVoidLambda

#if WINDOWS
using CommonHelpers.Collections;
using Microsoft.Graphics.Imaging;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.AI;
using Microsoft.Windows.AI.Imaging;
using System.Diagnostics;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using XkcdViewer.Windows.Utils;

namespace XkcdViewer.Maui.ViewModels;

public partial class MainPageViewModel
{
    private ImageDescriptionKind preferredDescriptionLevel = ImageDescriptionKind.DetailedDescription;

    public ObservableRangeCollection<ImageDescriptionKind> DescriptionLevels { get; } = [];

    public ImageDescriptionKind PreferredDescriptionLevel
    {
        get => preferredDescriptionLevel;
        set => SetProperty(ref preferredDescriptionLevel, value);
    }

    private void InitializeCopilotCapabilities()
    {
        // Light up Windows Foundry Gen AI capabilities if the system supports it
        if (AppUtils.HasNpu())
        {
            AreCopilotControlsVisible = true;
            DescriptionLevels.AddRange(Enum.GetValues<ImageDescriptionKind>());
            PreferredDescriptionLevel = DescriptionLevels.FirstOrDefault(n => n == ImageDescriptionKind.DetailedDescription);
        }
    }

    public async Task AnalyzeCurrentComicAsync()
    {
        if (IsBusy || CurrentComic is null || string.IsNullOrEmpty(CurrentComic.Img))
            return;

        try
        {
            IsBusy = true;
            IsBusyMessage = "Analyzing...";

            // - STEP 1 -
            // Download image and save to temp working file

            IsBusyMessage = "Downloading image...";
            await AppUtils.DownloadImageAsync(CurrentComic.Num, CurrentComic.Img);

            // - STEP 2 -
            // Load png file into a SoftwareBitmap

            IsBusyMessage = "Analyzing image...";
            var languageModelResponse = await GetImageDescriptionAsync(CurrentComic.Num);

            if (languageModelResponse == null)
                return;

            // - STEP 3 -
            // Audio playback using SpeechSynthesizer.

            IsBusyMessage = "Playing back audio...";
            await AppUtils.ReadAloudAsync(languageModelResponse.Description);
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

    private static void DeleteCachedComicImage(int comicId)
    {
        var filePath = AppUtils.IsPackagedApp
            ? Path.Combine(ApplicationData.Current.TemporaryFolder.Path, $"{comicId}.png")
            : Path.Combine(AppContext.BaseDirectory, $"{comicId}.png");

        if (!File.Exists(filePath))
            return;

        File.Delete(filePath);
    }

    private async Task<ImageDescriptionResult?> GetImageDescriptionAsync(int comicId)
    {
        var filePath = AppUtils.IsPackagedApp
            ? Path.Combine(ApplicationData.Current.TemporaryFolder.Path, $"{comicId}.png")
            : Path.Combine(AppContext.BaseDirectory, $"{comicId}.png");

        if (!File.Exists(filePath))
        {
            await ShowAnalyzerMessageAsync("Local image file does not exist, cannot proceed.");
            return null;
        }

        IRandomAccessStream stream;

        if (AppUtils.IsPackagedApp)
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

        // - STEP 1 -
        // Make sure Gen AI capabilities are onboard
        IsBusyMessage = "Checking Windows AI capabilities...";

        if (ImageDescriptionGenerator.GetReadyState() == AIFeatureReadyState.NotReady)
        {
            var wProg = ImageDescriptionGenerator.EnsureReadyAsync();

            wProg.Progress = (result, progress) =>
            {
                DispatcherQueue.GetForCurrentThread().TryEnqueue(() => IsBusyMessage = $"Downloading model... {progress * 100}% complete.");
            };

            AIFeatureReadyResult? result = await wProg;

            if (result.Status != AIFeatureReadyResultState.Failure)
            {
                await ShowAnalyzerMessageAsync($"There was a problem installing the required packages: {result.ExtendedError.Message}");
                return null;
            }
        }
        else if (ImageDescriptionGenerator.GetReadyState() == AIFeatureReadyState.NotSupportedOnCurrentSystem)
        {
            await ShowAnalyzerMessageAsync("This device does not support the required Gen AI capabilities.");
            return null;
        }

        // - STEP 2 -
        // Request an ImageDescriptionGenerator session from Windows AI Foundry
        IsBusyMessage = "Requesting Windows AI Foundry session...";
        var imageDescriptionGenerator = await ImageDescriptionGenerator.CreateAsync();

        // - STEP 3 -
        // Describe the image
        IsBusyMessage = "Preparing image...";
        var decoder = await BitmapDecoder.CreateAsync(stream);
        var sBitmap = await decoder.GetSoftwareBitmapAsync();

        IsBusyMessage = "Preparing AI...";
        var describer = imageDescriptionGenerator.DescribeAsync(
            ImageBuffer.CreateForSoftwareBitmap(sBitmap),
            ImageDescriptionKind.DetailedDescription,
            AppUtils.GetContentFilterOptions());

        describer.Progress = (info, streamingProgress) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsBusyMessage = $"Generating: {streamingProgress}";
            });
        };

        IsBusyMessage = "Analyzing image...";

        return await describer;
    }

    private async Task ShowAnalyzerMessageAsync(string message)
    {
        await App.Current.MainPage.DisplayAlert("Warning", message, "okay");
    }
}

#endif