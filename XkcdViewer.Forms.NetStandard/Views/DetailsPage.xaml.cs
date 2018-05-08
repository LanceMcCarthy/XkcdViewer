using Xamarin.Forms;
using XkcdViewer.Forms.NetStandard.Models;
using XkcdViewer.Forms.NetStandard.ViewModels;

namespace XkcdViewer.Forms.NetStandard.Views
{
    public partial class DetailsPage : ContentPage
    {
        private readonly DetailsPageViewModel vm;
         
        public DetailsPage(Comic comic)
        {
            InitializeComponent();
            vm = new DetailsPageViewModel(comic);
            BindingContext = vm;
        }
    }
}
