using CommonHelpers.Services;

namespace XkcdViewer.Maui;

public partial class App : Application
{
    public static XkcdApiService ApiService { get; set; }

    public App()
    {
        InitializeComponent();

        //MainPage = new AppShell();

        ApiService = new XkcdApiService();
        MainPage = new NavigationPage(new MainPage { IconImageSource = "ic_xkcd_light.png" });
    }
}