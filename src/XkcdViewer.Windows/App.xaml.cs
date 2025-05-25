using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace XkcdViewer.Windows;

public partial class App : Application
{
    private Window? mWindow;

    public App()
    {
        this.InitializeComponent();
    }

    public DispatcherQueue MainDispatcherQueue { get; private set; }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var window = new MainWindow();
        MainDispatcherQueue = window.DispatcherQueue;
        window.Activate();
    }
}