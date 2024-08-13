using XkcdViewer.Maui.ViewModels;

namespace XkcdViewer.Maui.Views;

public partial class FavoritesPage : BasePage
{
    public FavoritesPage(FavoritesPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}