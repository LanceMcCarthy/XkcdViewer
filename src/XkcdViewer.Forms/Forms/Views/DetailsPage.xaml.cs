using Xamarin.Forms;
using XkcdViewer.Forms.Models;

namespace XkcdViewer.Forms.Views
{
    public partial class DetailsPage : ContentPage
    {
        public DetailsPage(Comic comic)
        {
            InitializeComponent();
            ViewModel.SelectedComic = comic;
        }
    }
}
