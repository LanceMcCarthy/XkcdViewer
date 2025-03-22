using Microsoft.UI.Xaml;

namespace XkcdViewer.Windows;

public partial class App : Application
{
    private Window? mWindow;

    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        mWindow = new MainWindow();
        mWindow.Activate();
    }
}