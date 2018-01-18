using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using Telerik.XamarinForms.DataControls.ListView;
using Xamarin.Forms;
using XkcdViewer.Forms.NetStandard.Common;
using XkcdViewer.Forms.NetStandard.Models;
using XkcdViewer.Forms.NetStandard.Services;
using XkcdViewer.Forms.NetStandard.Views;

namespace XkcdViewer.Forms.NetStandard.ViewModels
{
    public class MainViewModel : PageBaseViewModel
    {
        private readonly NavigationService navigationService;
        private XkcdApiService apiService;
        private bool isInitialized;
        private int lastComicNumber;

        private ObservableCollection<Comic> comics;
        private Command loadDetailsCommand;
        private Command goToFavoritesCommand;
        private Command getComicCommand;
        private bool isFetching;

        public MainViewModel(NavigationService navService)
        {
            this.navigationService = navService;
            apiService = new XkcdApiService();
            GetNextComic();
            isInitialized = true;
        }
        
        public ObservableCollection<Comic> Comics
        {
            get => comics ?? (comics = new ObservableCollection<Comic>());
            set => Set(ref comics, value);
        }

        public bool IsFetching
        {
            get => isFetching;
            set => Set(ref isFetching, value);
        }

        public Command LoadDetailsCommand => loadDetailsCommand ?? (loadDetailsCommand = new Command(args =>
        {
            if ((args as ItemTapEventArgs)?.Item is Comic comic)
            {
                navigationService.Navigate(typeof(DetailsPage), comic);
            }
        }));

        public Command GoToFavoritesCommand => goToFavoritesCommand ?? (goToFavoritesCommand = new Command(() =>
        {
            navigationService.Navigate<FavoritesPage>();
        }));

        public Command GetComicCommand => getComicCommand ?? (getComicCommand = new Command((e) =>
        {
            if (e is PullToRefreshRequestedEventArgs args)
            {
                GetNextComic();
                args.Cancel = true;
            }
        }));
        
        public async void GetNextComic()
        {
            try
            {
                if(IsFetching)
                    return;
                
                IsFetching = IsBusy = true;

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
                IsFetching = IsBusy = false;
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
