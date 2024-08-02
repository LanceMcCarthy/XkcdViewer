using XkcdViewer.Maui.ViewModels;

namespace XkcdViewer.Maui.Views;

public partial class MainPage : BasePage
{
    private readonly MainPageViewModel pageViewModel;

    public MainPage(MainPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = pageViewModel = vm;
    }
}