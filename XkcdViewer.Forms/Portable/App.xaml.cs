using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portable.ViewModels;
using Portable.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Portable
{
    public partial class App : Application
    {
        public static MainViewModel ViewModel => ViewModelLocator.Main;

        public static BasePage RootPage { get; set; }

        public App()
        {
            RootPage = new BasePage(new MainPage());
            MainPage = RootPage;
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
