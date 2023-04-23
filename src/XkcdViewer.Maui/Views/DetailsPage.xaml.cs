using XkcdViewer.Maui.Models;

namespace XkcdViewer.Maui.Views;

public partial class DetailsPage : ContentPage
{
    public DetailsPage(Comic comic)
    {
        InitializeComponent();
        ViewModel.SelectedComic = comic;
    }
}