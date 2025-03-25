using CommonHelpers.Common;
using Microsoft.Graphics.Imaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AI.ContentModeration;
using Microsoft.Windows.AI.Generative;
using Microsoft.Windows.Management.Deployment;
using System;
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
using XkcdViewer.Windows.Utils;

namespace XkcdViewer.Windows;

public partial class MainViewModel : ViewModelBase
{
    private static readonly bool IsPackagedApp = (Environment.GetEnvironmentVariable("PACKAGED_PRODUCT_ID") != null);
    private static HttpClient? httpClient;
    private static MediaPlayerElement? mpe;

    private void InitializeCopilotCapabilities()
    {
        if (!AppUtils.HasNpu())
            return;

        // Shows the panel with Copilot controls
        CopilotCapVisibility = Visibility.Visible;
        
        // Populate the ComboBox with the available description levels
        var levelsList = Enum.GetValues<ImageDescriptionScenario>();
        DescriptionLevels.AddRange(levelsList);

        // Preselect the one I think works best
        PreferredDescriptionLevel = DescriptionLevels.FirstOrDefault(n => n == ImageDescriptionScenario.DetailedNarration);
    }

    public async Task AnalyzeCurrentComicAsync()
    {
        if (IsBusy || CurrentComic is null || string.IsNullOrEmpty(CurrentComic.Img))
            return;

        try
        {
            IsBusy = true;
            IsBusyMessage = "Analyzing...";

            // STEP 1 - Download image and save to temp working file
            IsBusyMessage = "Downloading image...";
            await DownloadImageAsync(CurrentComic.Num, CurrentComic.Img);

            // STEP 2 - Load png file into a SoftwareBitmap
            IsBusyMessage = "Analyzing image...";
            var languageModelResponse = await GetImageDescriptionAsync(CurrentComic.Num);

            if (languageModelResponse == null)
                return;

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

        IsBusyMessage = "Checking Windows AI capabilities...";

        if (!ImageDescriptionGenerator.IsAvailable())
        {
            IsBusyMessage = "Preparing ImageDescriptionGenerator for first time use...";

            var result = await ImageDescriptionGenerator.MakeAvailableAsync();

            if (result.Status != PackageDeploymentStatus.CompletedSuccess)
            {
                await ShowAnalyzerMessageAsync($"There was a problem installing the required packages: {result.ExtendedError.Message}");
                return null;
            }
        }

        // STEP 3 - Load the SoftwareBitmap into an ImageBuffer

        IsBusyMessage = "Preparing image buffer...";

        var inputImage = ImageBuffer.CreateCopyFromBitmap(sBitmap);

        if (inputImage is null)
        {
            await ShowAnalyzerMessageAsync("There was a problem creating the ImageBuffer from the SoftwareBitmap.");
            return null;
        }

        // STEP 4 - Use ImageDescriptionGenerator from Windows Gen AI APIs

        IsBusyMessage = "Requesting an ImageDescriptionGenerator for use...";

        var imageDescriptionGenerator = await ImageDescriptionGenerator.CreateAsync();

        IsBusyMessage = "Analyzing image...";

        var languageModelResponse = await imageDescriptionGenerator.DescribeAsync(
            inputImage,
            ImageDescriptionScenario.Accessibility,
            new ContentFilterOptions
            {
                PromptMinSeverityLevelToBlock = { ViolentContentSeverity = SeverityLevel.Medium },
                ResponseMinSeverityLevelToBlock = { ViolentContentSeverity = SeverityLevel.Medium }
            });

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
        mpe.MediaPlayer.Play();
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

        await this.DialogService.ShowDialogAsync(dialog);
    }
}