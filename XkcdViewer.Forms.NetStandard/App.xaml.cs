using GalaSoft.MvvmLight.Ioc;
using MonkeyCache.FileStore;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XkcdViewer.Forms.NetStandard.Common;
using XkcdViewer.Forms.NetStandard.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace XkcdViewer.Forms.NetStandard
{
    public partial class App : Application
    {
        private static NavigationService Navigation => SimpleIoc.Default.GetInstance<NavigationService>();
        
        public App()
        {
            InitializeComponent();
            MainPage = new ExtendedNavigationPage(Navigation, new MainPage { Icon = "ic_xkcd_light.png" });
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
