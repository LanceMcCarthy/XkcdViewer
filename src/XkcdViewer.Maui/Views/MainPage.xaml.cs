using XkcdViewer.Maui.ViewModels;

namespace XkcdViewer.Maui.Views;

public partial class MainPage : BasePage
{
    private readonly MainPageViewModel vm;

    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = vm = viewModel;
    }
}