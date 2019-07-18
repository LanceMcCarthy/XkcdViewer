using CommonHelpers.Common;
using XkcdViewer.Forms.Models;

namespace XkcdViewer.Forms.ViewModels
{
    public class DetailsPageViewModel : ViewModelBase
    {
        private Comic selectedComic;

        public DetailsPageViewModel()
        {
        }

        public Comic SelectedComic
        {
            get => selectedComic;
            set
            {
                if (SetProperty(ref selectedComic, value))
                {
                    this.Title = $"#{SelectedComic.Num}";
                }
            }
        }
    }
}