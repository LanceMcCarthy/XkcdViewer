using Xamarin.Forms;

namespace XkcdViewer
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
            
            NavigationPage.SetTitleIcon(page, "XKCD_small_logo.png");
        }
    }
}
