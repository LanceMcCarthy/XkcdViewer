using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using XkcdViewer.Forms.NetStandard.Models;

namespace XkcdViewer.Forms.NetStandard.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<Comic> comics;

        private int lastComicNumber;

        public MainViewModel()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                case Device.Android:
                    Title = "XKCD Viewer";
                    break;
                case Device.iOS:
                    Title = "XKCD Comic Viewer";
                    break;
            }
        }

        public ObservableCollection<Comic> Comics
        {
            get => comics ?? (comics = new ObservableCollection<Comic>());
            set => Set(ref comics, value);
        }

        public async Task GetNextComic()
        {
            try
            {
                if (IsBusy)
                    return;

                IsBusy = true;

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

                Comics.Add(comic);
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
    }
}