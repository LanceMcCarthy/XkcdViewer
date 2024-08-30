namespace XkcdViewer.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        this.Title = DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst ? "XKCD Comic Viewer" : "XKCD Viewer";
    }
}