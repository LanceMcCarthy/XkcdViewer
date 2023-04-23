using CommonHelpers.Common;
using XkcdViewer.Maui.Models;

namespace XkcdViewer.Maui.ViewModels;

public class DetailsPageViewModel : ViewModelBase
{
    private Comic selectedComic;

    public DetailsPageViewModel() { }

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