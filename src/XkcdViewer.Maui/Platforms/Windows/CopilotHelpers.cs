using Microsoft.Graphics.Imaging;
using Microsoft.Windows.AI.ContentModeration;
using Microsoft.Windows.AI.Generative;
using Microsoft.Windows.Management.Deployment;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;
using Microsoft.UI.Xaml.Media.Imaging;

namespace XkcdViewer.Maui.Platforms.Windows;

public static class CopilotHelpers
{
    public static async Task<string> DescribeImageAsync(SoftwareBitmap sBitmap)
    {
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

        // moderation threshold
        var filterOptions = new ContentFilterOptions();
        filterOptions.PromptMinSeverityLevelToBlock.ViolentContentSeverity = SeverityLevel.Medium;
        filterOptions.ResponseMinSeverityLevelToBlock.ViolentContentSeverity = SeverityLevel.Medium;

        // AI contents
        LanguageModelResponse languageModelResponse = await imageDescriptionGenerator.DescribeAsync(inputImage, ImageDescriptionScenario.DetailedNarration, filterOptions);
        string response = languageModelResponse.Response;
        return response;
    }

    public static async Task<SoftwareBitmapSource> ToSourceAsync(this SoftwareBitmap softwareBitmap)
    {
        var source = new SoftwareBitmapSource();

        if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 || softwareBitmap.BitmapAlphaMode != BitmapAlphaMode.Premultiplied)
        {
            var convertedBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            await source.SetBitmapAsync(convertedBitmap);
        }
        else
        {
            await source.SetBitmapAsync(softwareBitmap);
        }

        return source;
    }

    public static async Task<SoftwareBitmap> FilePathToSoftwareBitmapAsync(this string filePath)
    {
        using IRandomAccessStream stream = await CreateStreamAsync(filePath);
        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
        return await decoder.GetSoftwareBitmapAsync();
    }

    private static async Task<IRandomAccessStream> CreateStreamAsync(this string filepath)
    {
        if (string.IsNullOrWhiteSpace(filepath))
        {
            return MemoryStream.Null.AsRandomAccessStream();
        }

        if (IsPackagedApp)
        {
            StorageFile file = await CreateStorageFile(filepath);
            return await file.OpenAsync(FileAccessMode.Read);
        }
        else
        {
            string filePath = CombineWithBasePath(filepath);
            return File.OpenRead(filePath).AsRandomAccessStream();
        }
    }

    private static Task<StorageFile> CreateStorageFile(string filepath)
    {
        var uri = new Uri("ms-appx:///" + filepath);
        return StorageFile.GetFileFromApplicationUriAsync(uri).AsTask();
    }

    private static string CombineWithBasePath(string filepath)
    {
        return Path.Combine(AppContext.BaseDirectory, filepath);
    }

    private static bool IsPackagedApp = (Environment.GetEnvironmentVariable("PACKAGED_PRODUCT_ID") != null);
}
