using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PCLStorage;
using Xamarin.Forms;
using XkcdViewer.Models;
using XkcdViewer.ViewModels;

namespace XkcdViewer
{
	public class App : Application
	{
	    private static MainViewModel viewModel;
	    public static MainViewModel ViewModel => viewModel ?? (viewModel = new MainViewModel());
        
	    public App()
		{
            MainPage = new BasePage(new MainPage());
        }

		protected override void OnStart()
		{
            Debug.WriteLine($"------------OnStart() fired-----------");
        }

		protected override async void OnSleep()
		{
            Debug.WriteLine($"------------OnSleep() fired-----------");

		    if (ViewModel.FavoriteComics.Any())
                await ViewModel.SaveFavoritesAsync(ViewModel.FavoriteComics);
        }

		protected override void OnResume()
		{
            Debug.WriteLine("----------OnResume() Fired-----------");
        }
	}
}

