using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using ModernHttpClient;
using Newtonsoft.Json;
using Portable.Common;
using Portable.Models;
using Portable.Views;
using Xamarin.Forms;

namespace Portable.ViewModels
{
    public class MainViewModel : PageBaseViewModel
    {
        #region fields

        private readonly NavigationService navigationService;
        private readonly HttpClient client;

        private ObservableCollection<Comic> comics;
        private int lastComicNumber;
        private double progress;
        private Command<Comic> loadDetailsCommand;
        private Command goToFavoritesCommand;
        private Command getComicCommand;

        #endregion

        public MainViewModel(NavigationService navService)
        {
            this.navigationService = navService;

            InitializeViewModel();

            client = new HttpClient(new NativeMessageHandler());
            
        }

        #region properties

        public ObservableCollection<Comic> Comics
        {
            get { return comics ?? (comics = new ObservableCollection<Comic>()); }
            set { Set(ref comics, value); }
        }

        public double Progress
        {
            get { return progress; }
            set { Set(ref progress, value); }
        }

        #endregion

        #region methods

        private async void InitializeViewModel()
        {
            await GetComic();
        }

        /// <summary>
        /// Gets latest Comic from XKCD
        /// </summary>
        /// <returns></returns>
        public async Task GetComic()
        {
            try
            {
                IsBusy = true;
                var comic = await GetComicAsync(lastComicNumber);
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
        
        /// <summary>
        /// Gets a specific comic from XKCD
        /// </summary>
        /// <param name="comicNumber">Comic number</param>
        /// <returns></returns>
        private async Task<Comic> GetComicAsync(int comicNumber)
        {
            try
            {
                var url = lastComicNumber == 0 ? "https://xkcd.com/info.0.json" : "https://xkcd.com/" + $"{comicNumber - 1}" + "/info.0.json";
                
                using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseContentRead))
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(jsonResult))
                        return new Comic { Title = "Whoops", Transcript = $"There was no comic to be found" };

                    var result = JsonConvert.DeserializeObject<Comic>(jsonResult);

                    if (result == null)
                        return new Comic { Title = "Json Schmason", Transcript = $"Someone didnt like the way the comic's json tasted and spit it back out" };
                    
                    lastComicNumber = result.Num;

                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetComicJsonAsync Exception\r\n{ex}");
                return new Comic { Title = "Exception", Transcript = $"Error getting comic: {ex.Message}" };
            }
        }
        
        #endregion

        #region Commands

        /// <summary>
        /// Navigates to a new instance of DetailsPage and passes the SelectedComic to the DetailsPageViewModel
        /// </summary>
        public Command<Comic> LoadDetailsCommand => loadDetailsCommand ?? (loadDetailsCommand = new Command<Comic>( (comic) =>
        {
            if (comic == null)
                return;

            //var detailsPage = new DetailsPage();
            //var dpvm = detailsPage.BindingContext as DetailsPageViewModel;

            //if (dpvm != null)
            //    dpvm.SelectedComic = comic;

            //await App.RootPage.Navigation.PushAsync(detailsPage);

            this.navigationService.Navigate(typeof(DetailsPage), comic);
        }));

        public Command GoToFavoritesCommand => goToFavoritesCommand ?? (goToFavoritesCommand = new Command(() =>
        {
            //var favsPage = new FavoritesPage
            //{
            //    Title = "Favorites",
            //    Icon = ""
            //};

            //await App.RootPage.Navigation.PushAsync(favsPage);

            navigationService.Navigate<FavoritesPage>();
        }));

        public Command GetComicCommand => getComicCommand ?? (getComicCommand = new Command(async () =>
        {
            await GetComic();
        }));

        #endregion

        #region event handlers

        private void ProgressReporter_ProgressChanged(object sender, DownloadProgressArgs args)
        {
            this.Progress = (int) args.PercentComplete;
        }

        #endregion

        public override Task OnNavigatedToAsync(NavigationServiceNavigationEventArgs eventArgs)
        {
            return base.OnNavigatedToAsync(eventArgs);
        }

        public override Task OnNavigatedFromAsync(NavigationServiceNavigationEventArgs eventArgs)
        {
            return base.OnNavigatedFromAsync(eventArgs);
        }
    }
}
