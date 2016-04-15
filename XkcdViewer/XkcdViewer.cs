using Xamarin.Forms;
using XkcdViewer.ViewModels;

namespace XkcdViewer
{
	public class App : Application
	{
	    private static MainViewModel viewModel;
	    public static MainViewModel ViewModel => viewModel ?? (viewModel = new MainViewModel());


	    public App ()
		{
            //MainPage = new MainPage();

            MainPage = new NavigationPage(new MainPage());
        }

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

