using Xamarin.Forms;
using XkcdViewer.Forms.NetStandard.Models;

namespace XkcdViewer.Forms.NetStandard.Views
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
