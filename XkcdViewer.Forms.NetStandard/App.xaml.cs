using MonkeyCache.FileStore;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XkcdViewer.Forms.NetStandard.Services;
using XkcdViewer.Forms.NetStandard.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace XkcdViewer.Forms.NetStandard
{
    public partial class App : Application
    {
        public static XkcdApiService ApiService { get; set; }

        public App()
        {
            InitializeComponent();
            ApiService = new XkcdApiService();
            MainPage = new NavigationPage(new MainPage { Icon = "ic_xkcd_light.png" });
        }
        
        protected override void OnStart()
        {
            Barrel.ApplicationId = "xkcd_viewer";
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
