using XkcdViewer.Maui.ViewModels;

namespace XkcdViewer.Maui.Views;

public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel pageViewModel;

    public MainPage(MainPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = pageViewModel = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (pageViewModel.Comics == null || pageViewModel.Comics.Count == 0)
        {
            //await pageViewModel.FetchComic();

            await pageViewModel.OnAppearing();
        }
    }
}