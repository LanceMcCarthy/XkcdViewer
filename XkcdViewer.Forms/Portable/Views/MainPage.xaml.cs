using Portable.Common;
using Telerik.XamarinForms.DataControls.ListView;

namespace Portable.Views
{
    public partial class MainPage : PageBase
    {
        public MainPage()
        {
            InitializeComponent();
        }

        //private void FavoriteButtonClicked(object sender, EventArgs e)
        //{
        //    var button = sender as Button;
        //    var selectedComic = button?.BindingContext as Comic;
        //    if (selectedComic == null) return;

        //    App.ViewModel.FavoriteComics.Add(selectedComic);

        //    MyRadListView.EndItemSwipe();
        //}

        //private async void MyRadListView_OnRefreshRequested(object sender, PullToRefreshRequestedEventArgs e)
        //{
        //    await App.ViewModel.GetComic();
        //    MyRadListView.EndRefresh();
        //}
    }
}

