using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portable.ViewModels;
using Portable.Views;
using Xamarin.Forms;

namespace Portable
{
    public partial class App : Application
    {
        private static MainViewModel viewModel;
        public static MainViewModel ViewModel => viewModel ?? (viewModel = new MainViewModel());

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
