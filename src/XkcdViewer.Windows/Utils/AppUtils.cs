using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.DXCore;

namespace XkcdViewer.Windows.Utils;

public static class AppUtils
{
    private static readonly Guid DXCORE_ADAPTER_ATTRIBUTE_D3D12_GENERIC_ML = new(0xb71b0d41, 0x1088, 0x422f, 0xa2, 0x7c, 0x2, 0x50, 0xb7, 0xd3, 0xa9, 0x88);
    private static bool? hasNpu;

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
}