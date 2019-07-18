using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XkcdViewer.Forms.Services;
using XkcdViewer.Forms.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace XkcdViewer.Forms
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
        
        protected override void OnStart() { }

        protected override void OnSleep() { }

        protected override void OnResume() { }
    }
}
