using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ModernHttpClient;
using Newtonsoft.Json;
using XkcdViewer.Annotations;
using XkcdViewer.Models;

namespace XkcdViewer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region fields
        
        private ObservableCollection<Comic> comics;
        private bool isBusy;

        private int lastComicNumber;
        private ObservableCollection<Comic> favoriteComics;

        #endregion

        public MainViewModel()
        {

        }

        #region properties
        
        public ObservableCollection<Comic> Comics
        {
            get { return comics ?? (comics = new ObservableCollection<Comic>()); }
            set { comics = value; OnPropertyChanged();}
        }

        public ObservableCollection<Comic> FavoriteComics
        {
            get { return favoriteComics ?? (favoriteComics = new ObservableCollection<Comic>()); }
            set { favoriteComics = value; }
        }

        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; OnPropertyChanged(); }
        }
        
        #endregion

        #region methods

        public async Task GetComic()
        {
            try
            {
                IsBusy = true;
                Comics.Insert(0, await GetComicAsync(lastComicNumber));
                //Comics.Add(await GetComicAsync(lastComicNumber));
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
        /// Gets a specific comic from Xkcd using the comic number
        /// </summary>
        /// <param name="comicNumber">the number of the comic to get</param>
        /// <returns></returns>
        private async Task<Comic> GetComicAsync(int comicNumber)
        {
            try
            {
                string jsonResult;
                string url = lastComicNumber == 0 ? "https://xkcd.com/info.0.json" : "https://xkcd.com/" + $"{comicNumber - 1}" + "/info.0.json";
                
                using (var client = new HttpClient(new NativeMessageHandler()))
                {
                    jsonResult = await client.GetStringAsync(url);
                }

                if (string.IsNullOrEmpty(jsonResult))
                {
                    Debug.WriteLine("There were no results");
                    return new Comic();
                }

                var result = JsonConvert.DeserializeObject<Comic>(jsonResult);

                if (result == null)
                    return new Comic {Title = "No Result"};

                lastComicNumber = result.Num;
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetComicJsonAsync Exception\r\n{ex}");
                return new Comic { Title = "Exception", Transcript = $"Error getting comic: {ex.Message}"};
            }
        }

        #endregion

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
