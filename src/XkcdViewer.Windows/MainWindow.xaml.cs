using Microsoft.Graphics.Imaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AI.ContentModeration;
using Microsoft.Windows.AI.Generative;
using Microsoft.Windows.Management.Deployment;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using Windows.Graphics.Imaging;
using Windows.Media.Core;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.Storage.Streams;

namespace XkcdViewer.Windows;

public sealed partial class MainWindow : Window
{
    private static bool IsPackagedApp = (Environment.GetEnvironmentVariable("PACKAGED_PRODUCT_ID") != null);

    public MainWindow()
    {
        InitializeComponent();
        RootGrid.Loaded += (s, e) => ((MainViewModel)((Grid)s).DataContext)?.InitialLoadAsync();
    }

    private async void AnalyzeButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (ViewModel.CurrentComic?.Img is null)
            return;

        try
        {
            ViewModel.IsBusy = true;
            ViewModel.IsBusyMessage = "Analyzing Image...";

            var fileName = "analyze.png";
            var filePath = IsPackagedApp ? "ms-appdata:///local/analyze.png" : Path.Combine(AppContext.BaseDirectory, fileName);

            // Download and save to working file
            ViewModel.IsBusyMessage = "Downloading image...";
            var httpClient = new HttpClient();
            var imageBytes = await httpClient.GetByteArrayAsync(ViewModel.CurrentComic.Img);
            await File.WriteAllBytesAsync(filePath, imageBytes);


            // STEP 2 - Convert png to SoftwareBitmap
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

            var decoder = await BitmapDecoder.CreateAsync(stream);
            var sBitmap = await decoder.GetSoftwareBitmapAsync();


            // STEP 3 - Use new AI chops
            ViewModel.IsBusyMessage = "Checking for Copilot+ NPU capabilities...";

            if (!ImageDescriptionGenerator.IsAvailable())
            {
                var result = await ImageDescriptionGenerator.MakeAvailableAsync();
                if (result.Status != PackageDeploymentStatus.CompletedSuccess)
                {
                    throw result.ExtendedError;
                }
            }

            var imageDescriptionGenerator = await ImageDescriptionGenerator.CreateAsync();
            var inputImage = ImageBuffer.CreateCopyFromBitmap(sBitmap);

            var filterOptions = new ContentFilterOptions
            {
                PromptMinSeverityLevelToBlock = { ViolentContentSeverity = SeverityLevel.Medium },
                ResponseMinSeverityLevelToBlock = { ViolentContentSeverity = SeverityLevel.Medium }
            };

            var languageModelResponse = await imageDescriptionGenerator.DescribeAsync(inputImage, ImageDescriptionScenario.DetailedNarration, filterOptions);
            var response = languageModelResponse.Response;



            // STEP 4 - Audio Playback of AI description

            ViewModel.IsBusyMessage = "Playing back audio...";

            var synthesizer = new SpeechSynthesizer();
            var synthesisStream = await synthesizer.SynthesizeTextToStreamAsync(response);

            var mediaPlayerElement = new MediaPlayerElement();
            var mediaSource = MediaSource.CreateFromStream(synthesisStream, synthesisStream.ContentType);
            mediaPlayerElement.Source = mediaSource;
            mediaPlayerElement.MediaPlayer.Play();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        finally
        {

            ViewModel.IsBusyMessage = "";
            ViewModel.IsBusy = false;
        }
    }
}