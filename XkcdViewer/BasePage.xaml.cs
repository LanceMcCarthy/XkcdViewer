using Xamarin.Forms;

namespace XkcdViewer
{
    public partial class BasePage : NavigationPage
    {
        public BasePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Instantiate the root NavigationPage with the first ContentPage
        /// </summary>
        /// <param name="page">ContentPage to be shown at launch</param>
        public BasePage(Page page)
        {
            InitializeComponent();

            //push the passed Page into the nav stack
            this.Navigation.PushAsync(page);

            //Sets the TitleBar icon
            NavigationPage.SetTitleIcon(page, "XKCD_small_logo.png");
        }
    }
}
