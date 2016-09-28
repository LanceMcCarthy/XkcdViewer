using Portable.Models;
using Portable.ViewModels;
using Xamarin.Forms;

namespace Portable.Views
{
    public partial class DetailsPage : ContentPage
    {
        public DetailsPage()
        {
            InitializeComponent();
            //(BindingContext as DetailsPageViewModel).SelectedComic = selectedComic;
        }
    }
}
