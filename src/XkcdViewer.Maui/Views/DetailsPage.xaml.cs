using XkcdViewer.Maui.Models;
using XkcdViewer.Maui.ViewModels;

namespace XkcdViewer.Maui.Views;

public partial class DetailsPage : ContentPage
{
    public DetailsPage(DetailsPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}