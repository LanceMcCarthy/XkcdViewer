using XkcdViewer.Maui.Models;
using XkcdViewer.Maui.ViewModels;

namespace XkcdViewer.Maui.Views;

public partial class MainPage : ContentPage, ICollectionViewPage
{
    private readonly MainViewModel viewModel;

    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = viewModel = vm;
        viewModel.CollectionViewPage = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (viewModel.Comics.Count == 0)
        {
            await viewModel.GetNextComic();
        }
    }

    public void ScrollIntoView(object item, bool isAnimated) => CollectionView1.ScrollItemIntoView(item, isAnimated);
}