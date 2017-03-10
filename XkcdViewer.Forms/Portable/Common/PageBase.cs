using Xamarin.Forms;

namespace Portable.Common
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
