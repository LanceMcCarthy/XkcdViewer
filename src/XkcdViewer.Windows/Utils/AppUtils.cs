using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AI.ContentSafety;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.DXCore;

namespace XkcdViewer.Windows.Utils;

public static class AppUtils
{
    private static readonly Guid DXCORE_ADAPTER_ATTRIBUTE_D3D12_GENERIC_ML = new(0xb71b0d41, 0x1088, 0x422f, 0xa2, 0x7c, 0x2, 0x50, 0xb7, 0xd3, 0xa9, 0x88);
    private static bool? hasNpu;
    private static HttpClient? httpClient;
    private static MediaPlayerElement? mpe;

    public static readonly bool IsPackagedApp = (Environment.GetEnvironmentVariable("PACKAGED_PRODUCT_ID") != null);

    public static bool HasNpu()
    {
        if (hasNpu.HasValue)
            return hasNpu.Value;

        if (PInvoke.DXCoreCreateAdapterFactory(typeof(IDXCoreAdapterFactory).GUID, out var adapterFactoryObj) != HRESULT.S_OK)
        {
            throw new InvalidOperationException("Failed to create adapter factory");
        }

        var adapterFactory = (IDXCoreAdapterFactory)adapterFactoryObj;

        // First try getting all GENERIC_ML devices, which is the broadest set of adapters and includes both GPUs and NPUs;
        // WARNING: older Windows versions may not have drivers that report GENERIC_ML.
        adapterFactory.CreateAdapterList([DXCORE_ADAPTER_ATTRIBUTE_D3D12_GENERIC_ML], typeof(IDXCoreAdapterList).GUID, out var adapterListObj);
        var adapterList = (IDXCoreAdapterList)adapterListObj;

        // Fall back to CORE_COMPUTE if GENERIC_ML devices are not available. This is a more restricted
        // set of adapters and may filter out some NPUs.
        if (adapterList.GetAdapterCount() == 0)
        {
            adapterFactory.CreateAdapterList(
                [PInvoke.DXCORE_ADAPTER_ATTRIBUTE_D3D12_CORE_COMPUTE],
                typeof(IDXCoreAdapterList).GUID,
                out adapterListObj);

            adapterList = (IDXCoreAdapterList)adapterListObj;
        }

        if (adapterList.GetAdapterCount() == 0)
        {
            throw new InvalidOperationException("No compatible adapters found.");
        }

        // Sort the adapters by preference, with hardware and high-performance adapters first.
        ReadOnlySpan<DXCoreAdapterPreference> preferences =
        [
            DXCoreAdapterPreference.Hardware,
            DXCoreAdapterPreference.HighPerformance
        ];

        adapterList.Sort(preferences);

        List<IDXCoreAdapter> adapters = [];

        for (uint i = 0; i < adapterList.GetAdapterCount(); i++)
        {
            adapterList.GetAdapter(i, typeof(IDXCoreAdapter).GUID, out var adapterObj);

            var adapter = (IDXCoreAdapter)adapterObj;

            adapter.GetPropertySize(
                DXCoreAdapterProperty.DriverDescription,
                out var descriptionSize);

            string adapterDescription;

            var buffer = nint.Zero;

            try
            {
                buffer = Marshal.AllocHGlobal((int)descriptionSize);

                unsafe
                {
                    adapter.GetProperty(
                        DXCoreAdapterProperty.DriverDescription,
                        descriptionSize,
                        buffer.ToPointer());
                }

                adapterDescription = Marshal.PtrToStringAnsi(buffer) ?? string.Empty;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }

            // Remove trailing null terminator written by DXCore.
            while (!string.IsNullOrEmpty(adapterDescription) && adapterDescription[^1] == '\0')
            {
                adapterDescription = adapterDescription[..^1];
            }

            adapters.Add(adapter);

            if (adapterDescription.Contains("NPU"))
            {
                hasNpu = true;
                return true;
            }
        }

        hasNpu = false;
        return false;
    }

    public static async Task DownloadImageAsync(int comicId, string url)
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

    public static async Task ReadAloudAsync(string message)
    {
        var synthesizer = new SpeechSynthesizer();
        var synthesisStream = await synthesizer.SynthesizeTextToStreamAsync(message);

        if (synthesisStream is null)
        {
            //await ShowAnalyzerMessageAsync("Could not generate synthesized audio.");
            return;
        }

        mpe ??= new MediaPlayerElement();
        var mediaSource = MediaSource.CreateFromStream(synthesisStream, synthesisStream.ContentType);
        mpe.Source = mediaSource;

        mpe.MediaPlayer.Play();
    }

    public static ContentFilterOptions GetContentFilterOptions()
    {
        return new ContentFilterOptions
        {
            ImageMaxAllowedSeverityLevel = new ImageContentFilterSeverity(SeverityLevel.Medium),
            PromptMaxAllowedSeverityLevel =
            {
                Sexual = SeverityLevel.Medium,
                Hate = SeverityLevel.Minimum,
                SelfHarm = SeverityLevel.Minimum,
                Violent = SeverityLevel.Minimum
            },
            ResponseMaxAllowedSeverityLevel =
            {
                Sexual = SeverityLevel.Medium,
                Hate = SeverityLevel.Minimum,
                SelfHarm = SeverityLevel.Minimum,
                Violent = SeverityLevel.Minimum
            }
        };
    }
}