using Xamarin.Forms;

namespace Portable
{
    public partial class BasePage : NavigationPage
    {
        public BasePage()
        {
            InitializeComponent();
        }

        public BasePage(Page page)
        {
            InitializeComponent();
            
            this.Navigation.PushAsync(page);
            
            NavigationPage.SetTitleIcon(page, "ic_xkcd_light.png");
        }
    }
}
