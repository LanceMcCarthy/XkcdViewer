using CommonHelpers.Common;
using CommonHelpers.Models;
using System.Collections.ObjectModel;
using Telerik.Maui.Controls.Compatibility.DataControls.ListView;
using XkcdViewer.Maui.Models;
using XkcdViewer.Maui.Services;
using XkcdViewer.Maui.ViewModels;
using IViewModel = XkcdViewer.Maui.Interfaces.IViewModel;

namespace XkcdViewer.Maui.Views;

public partial class FavoritesPage : BasePage
{
    private readonly FavoritesPageViewModel viewModel;

    public FavoritesPage(FavoritesPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = viewModel = vm;
    }

    //private async void Lv_ItemTapped(object sender, ItemTapEventArgs e)
    //{
    //    await Shell.Current.GoToAsync("/Details", new Dictionary<string, object>
    //    {
    //        { "SelectedComic", e.Item }
    //    });
    //}
}