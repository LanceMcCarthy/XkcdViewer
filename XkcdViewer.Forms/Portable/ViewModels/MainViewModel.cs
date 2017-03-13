using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using Portable.Common;
using Portable.Models;
using Portable.Services;
using Portable.Views;
using Xamarin.Forms;

namespace Portable.ViewModels
{
    public class MainViewModel : PageBaseViewModel
    {
        private readonly NavigationService navigationService;
        private XkcdApiService apiService;
        private bool isInitialized;
        private int lastComicNumber;

        private ObservableCollection<Comic> comics;
        private Command<Comic> loadDetailsCommand;
        private Command goToFavoritesCommand;
        private Command getComicCommand;
        
        public MainViewModel(NavigationService navService)
        {
            this.navigationService = navService;
            apiService = new XkcdApiService();
            GetNextComic();
            isInitialized = true;
        }
        
        public ObservableCollection<Comic> Comics
        {
            get { return comics ?? (comics = new ObservableCollection<Comic>()); }
            set { Set(ref comics, value); }
        }
        
        public Command<Comic> LoadDetailsCommand => loadDetailsCommand ?? (loadDetailsCommand = new Command<Comic>( (comic) =>
        {
            if (comic != null)
                navigationService.Navigate(typeof(DetailsPage), comic);
        }));

        public Command GoToFavoritesCommand => goToFavoritesCommand ?? (goToFavoritesCommand = new Command(() =>
        {
            navigationService.Navigate<FavoritesPage>();
        }));

        public Command GetComicCommand => getComicCommand ?? (getComicCommand = new Command(GetNextComic));
        
        public async void GetNextComic()
        {
            try
            {
                IsBusy = true;

                Comic comic;

                if (lastComicNumber == 0)
                {
                    comic = await apiService.GetNewestComicAsync();
                }
                else
                {
                    comic = await apiService.GetComicAsync(lastComicNumber - 1);
                }

                lastComicNumber = comic.Num;
                Comics.Insert(0, comic);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadComicsAsync Exception\r\n{ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        public override Task OnNavigatedToAsync(NavigationServiceNavigationEventArgs eventArgs)
        {
            if (!isInitialized)
            {
                apiService = new XkcdApiService();
                GetNextComic();
                isInitialized = true;
            }

            return base.OnNavigatedToAsync(eventArgs);
        }

        public override Task OnNavigatedFromAsync(NavigationServiceNavigationEventArgs eventArgs)
        {
            return base.OnNavigatedFromAsync(eventArgs);
        }
    }
}
