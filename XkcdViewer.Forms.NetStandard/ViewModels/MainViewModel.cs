using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Telerik.XamarinForms.DataControls.ListView;
using Xamarin.Forms;
using XkcdViewer.Forms.NetStandard.Common;
using XkcdViewer.Forms.NetStandard.Models;
using XkcdViewer.Forms.NetStandard.Services;
using XkcdViewer.Forms.NetStandard.Views;

namespace XkcdViewer.Forms.NetStandard.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private bool isInitialized;
        private int lastComicNumber;

        private ObservableCollection<Comic> comics;
        private Command getComicCommand;
        private bool isFetching;

        public MainViewModel()
        {
        }

        public async Task IntializeViewModel()
        {
            if (!isInitialized)
            {
                await GetNextComic();
                isInitialized = true;
            }
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
        
        public Command GetComicCommand => getComicCommand ?? (getComicCommand = new Command(async (e) =>
        {
            if (e is PullToRefreshRequestedEventArgs args)
            {
                await GetNextComic();
                args.Cancel = true;
            }
        }));
        
        public async Task GetNextComic()
        {
            try
            {
                if(IsFetching)
                    return;
                
                IsFetching = IsBusy = true;

                Comic comic;

                if (lastComicNumber == 0)
                {
                    comic = await App.ApiService.GetNewestComicAsync();
                }
                else
                {
                    comic = await App.ApiService.GetComicAsync(lastComicNumber - 1);
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
    }
}
