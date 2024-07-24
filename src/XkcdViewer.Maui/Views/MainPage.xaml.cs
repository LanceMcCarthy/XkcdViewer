using Telerik.Maui.Controls.Compatibility.DataControls.ListView;
using XkcdViewer.Maui.ViewModels;

namespace XkcdViewer.Maui.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel viewModel;

    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = viewModel = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (viewModel.Comics.Count == 0)
        {
            await viewModel.GetNextComic();
        }
    }

    private async void ListView_LoadOnDemand(object sender, EventArgs e)
    {
        await viewModel.GetNextComic();
    }

    private async void Lv_ItemTapped(object sender, ItemTapEventArgs e)
    {
        await Shell.Current.GoToAsync("/Favorites", new Dictionary<string, object>
        {
            {"SelectedComic", e.Item}
        });
    }
}