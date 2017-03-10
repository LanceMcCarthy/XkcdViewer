using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Ioc;
using Portable.Common;
using Portable.ViewModels;
using Portable.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Portable
{
    public partial class App : Application
    {
        private static INavigationService Navigation => SimpleIoc.Default.GetInstance<INavigationService>();
        
        public App()
        {
            MainPage = new ExtendedNavigationPage(Navigation, new MainPage { Icon = "ic_xkcd_light.png" });
        }
        

        protected override void OnStart()
        {
            // Handle when your app starts
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
