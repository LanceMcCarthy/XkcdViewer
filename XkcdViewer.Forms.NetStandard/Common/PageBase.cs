using Xamarin.Forms;

namespace XkcdViewer.Forms.NetStandard.Common
{
    public class PageBase : ContentPage
    {
        public object NavigationParameter { get; private set; }

        public void SetNavigationParameter(object parameter)
        {
            NavigationParameter = parameter;
        }
    }
}
