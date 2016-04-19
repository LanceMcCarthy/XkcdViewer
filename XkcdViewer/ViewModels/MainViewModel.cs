using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ModernHttpClient;
using Newtonsoft.Json;
using PCLStorage;
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

        private bool gettingComic;
        private double progress;

        #endregion

        public MainViewModel()
        {
            LoadData();
        }

        private async void LoadData()
        {
            var loadedFavs = await LoadFavoritesAsync();

            if (loadedFavs != null)
                FavoriteComics = loadedFavs;

            //Get the latest comic to start'er up
            await App.ViewModel.GetComic();
        }

        #region properties

        public ObservableCollection<Comic> Comics
        {
            get { return comics ?? (comics = new ObservableCollection<Comic>()); }
            set { comics = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Comic> FavoriteComics
        {
            get { return favoriteComics ?? (favoriteComics = new ObservableCollection<Comic>()); }
            set { favoriteComics = value; }
        }

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy == value) return;
                isBusy = value;
                OnPropertyChanged();
            }
        }

        public double Progress
        {
            get { return progress; }
            set { progress = value; OnPropertyChanged(); }
        }

        #endregion

        #region methods

        /// <summary>
        /// Gets latest Comic from XKCD
        /// </summary>
        /// <returns></returns>
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
        /// Gets a specific comic from XKCD
        /// </summary>
        /// <param name="comicNumber">Comic number</param>
        /// <returns></returns>
        private async Task<Comic> GetComicAsync(int comicNumber)
        {
            try
            {
                string jsonResult;
                string url = lastComicNumber == 0 ? "https://xkcd.com/info.0.json" : "https://xkcd.com/" + $"{comicNumber - 1}" + "/info.0.json";

                //TODO temporary commented out so that I can test download progress approach
                //using (var client = new HttpClient(new NativeMessageHandler()))
                //{
                //    jsonResult = await client.GetStringAsync(url);
                //}

                //--------------temp-----------------//

                this.Progress = 0;

                var progressReporter = new Progress<DownloadBytesProgress>();

                progressReporter.ProgressChanged += (s, args) =>
                {
                    this.Progress = (int) (100 * args.PercentComplete);
                };

                jsonResult = await DownloadStringWithProgressAsync(url, progressReporter);

                //----------------------------------//

                if (string.IsNullOrEmpty(jsonResult))
                {
                    Debug.WriteLine("There were no results");
                    return new Comic();
                }

                var result = JsonConvert.DeserializeObject<Comic>(jsonResult);

                if (result == null)
                    return new Comic { Title = "No Result" };

                lastComicNumber = result.Num;
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetComicJsonAsync Exception\r\n{ex}");
                return new Comic { Title = "Exception", Transcript = $"Error getting comic: {ex.Message}" };
            }
        }

        public async Task<bool> SaveFavoritesAsync(ObservableCollection<Comic> favs)
        {
            try
            {
                Debug.WriteLine($"---SaveFavoritesAsync called----");

                IsBusy = true;

                //using PCLStorage
                var favsAsJson = JsonConvert.SerializeObject(favs);
                var file = await FileSystem.Current.LocalStorage.CreateFileAsync("FavoritesJsonData.txt", CreationCollisionOption.ReplaceExisting);
                await file.WriteAllTextAsync(favsAsJson);

                Debug.WriteLine($"---SaveFavoritesAsync: {favs.Count} favorites saved");

                return true;

                //using DependencyService
                //var favsAsJson = JsonConvert.SerializeObject(ViewModel.FavoriteComics);
                //DependencyService.Get<ISaveAndLoad>().SaveText("FavoritesJsonData.txt", favsAsJson);
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"SaveFavoritesAsync JSONException: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SaveFavoritesAsync Exception: {ex}");
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<ObservableCollection<Comic>> LoadFavoritesAsync()
        {
            try
            {
                Debug.WriteLine($"---LoadFavoritesAsync called----");

                IsBusy = true;

                //using PCLStorage implementaiotn
                var file = await FileSystem.Current.LocalStorage.GetFileAsync("FavoritesJsonData.txt");

                var favsAsJson = await file.ReadAllTextAsync();
                var favsCollection = JsonConvert.DeserializeObject<ObservableCollection<Comic>>(favsAsJson);

                Debug.WriteLine($"---LoadFavoritesAsync: {favsCollection.Count} favorites loaded");

                return favsCollection;

                //using DependencyService
                //var favsAsJson = DependencyService.Get<ISaveAndLoad>().LoadText("FavoritesJsonData.txt");
                //var favsCollection = JsonConvert.DeserializeObject<ObservableCollection<Comic>>(favsAsJson);
                //ViewModel.FavoriteComics = favsCollection;
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"LoadFavoritesAsync JSONException: {ex}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadFavoritesAsync Exception: {ex}");
                return null;
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        private async Task<string> DownloadStringWithProgressAsync(string urlToGet, IProgress<DownloadBytesProgress> progessReporter)
        {
            int receivedBytes = 0;

            //
            
            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                client.DefaultRequestHeaders.ExpectContinue = false;

                var response = await client.GetAsync(urlToGet, HttpCompletionOption.ResponseHeadersRead); 
                
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var totalBytes = Convert.ToInt32(response.Content.Headers.ContentLength);

                    while (true)
                    {
                        var buffer = new byte[4096];
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                        if (bytesRead == 0)
                        {
                            await Task.Yield();
                            break;
                        }

                        receivedBytes += bytesRead;

                        if (progessReporter != null)
                        {
                            var args = new DownloadBytesProgress(urlToGet, receivedBytes, totalBytes);
                            progessReporter.Report(args);
                        }

                        Debug.WriteLine("Bytes read: {0}", bytesRead);
                    }

                    Debug.WriteLine("TOTAL Bytes read: {0}", receivedBytes);

                    stream.Position = 0;
                    var stringContent = new StreamReader(stream);
                    return stringContent.ReadToEnd();
                }
            }
        }
        
        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class DownloadBytesProgress
    {
        public DownloadBytesProgress(string fileName, int bytesReceived, int totalBytes)
        {
            Filename = fileName;
            BytesReceived = bytesReceived;
            TotalBytes = totalBytes;
        }

        public int TotalBytes { get; private set; }

        public int BytesReceived { get; private set; }

        public float PercentComplete { get { return (float) BytesReceived / TotalBytes; } }

        public string Filename { get; private set; }

        public bool IsFinished { get { return BytesReceived == TotalBytes; } }
    }
}
