using XkcdViewer.Maui.Views;

namespace XkcdViewer.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("Home/Details", typeof(DetailsPage));
        Routing.RegisterRoute("Favorites/Details", typeof(DetailsPage));
    }
}