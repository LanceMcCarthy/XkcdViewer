using XkcdViewer.Maui.ViewModels;

namespace XkcdViewer.Maui.Views;

public partial class MainPage : BasePage
{
    public MainPage(MainPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}