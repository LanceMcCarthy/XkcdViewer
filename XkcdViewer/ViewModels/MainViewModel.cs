using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
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
        private ObservableCollection<Comic> favoriteComics;
        private bool isBusy;
        private int lastComicNumber;
        
        private double progress;

        #endregion

        public MainViewModel()
        {
            InitializeViewModel();
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
            set { isBusy = value; OnPropertyChanged(); }
        }

        public double Progress
        {
            get { return progress; }
            set { progress = value; OnPropertyChanged(); }
        }

        #endregion

        #region methods

        /// <summary>
        /// Runs any neccessary utilities and methods when MainViewModel is instantiated
        /// </summary>
        private async void InitializeViewModel()
        {
            var loadedFavs = await LoadFavoritesAsync();

            if (loadedFavs != null)
                FavoriteComics = loadedFavs;

            //Get the latest comic to start'er up
            await App.ViewModel.GetComic();
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
        /// Stand-in replacement for HttpClient.GetStringAsync. This will report download progress.
        /// </summary>
        /// <param name="url">Url of where to download the string from</param>
        /// <param name="progessReporter">Args for reporting progress of the download operation</param>
        /// <returns>String result of the GET request (can be json or text)</returns>
        private static async Task<string> DownloadStringWithProgressAsync(string url, IProgress<DownloadProgressArgs> progessReporter)
        {
            try
            {
                using (var client = new HttpClient(new NativeMessageHandler()))
                {
                    client.DefaultRequestHeaders.ExpectContinue = false;

                    var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                    await response.Content.LoadIntoBufferAsync();

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        int receivedBytes = 0;
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
                                var args = new DownloadProgressArgs(receivedBytes, totalBytes);
                                progessReporter.Report(args);
                            }

                            Debug.WriteLine($"INCREMENTAL - Bytes read: {bytesRead}");
                        }

                        Debug.WriteLine($"TOTAL - Bytes read: {receivedBytes}");

                        stream.Position = 0;
                        var stringContent = new StreamReader(stream);
                        return stringContent.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DownloadStringWithProgressAsync Exception\r\n{ex}");
                return null;
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
                
                //reset the Progress
                Progress = 0;

                //--- download comic json, with progress reports ---//
                var progressReporter = new Progress<DownloadProgressArgs>();
                progressReporter.ProgressChanged += ProgressReporter_ProgressChanged;
                
                var jsonResult = await DownloadStringWithProgressAsync(url, progressReporter);

                progressReporter.ProgressChanged -= ProgressReporter_ProgressChanged;


                if (string.IsNullOrEmpty(jsonResult))
                    return new Comic { Title = "Whoops", Transcript = $"There was no XKCD comic to be found here" };

                var result = JsonConvert.DeserializeObject<Comic>(jsonResult);

                if (result == null)
                    return new Comic { Title = "Json Schmason", Transcript = $"Someone didnt like the way the comic's json tasted and spit it back out" };

                //keep track of what comic number we got last
                lastComicNumber = result.Num;

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetComicJsonAsync Exception\r\n{ex}");
                return new Comic { Title = "Exception", Transcript = $"Error getting comic: {ex.Message}" };
            }
        }
        
        /// <summary>
        /// Saves Favorites collection to phone storage
        /// </summary>
        /// <param name="favs">Favorites collection to save</param>
        /// <returns></returns>
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

        /// <summary>
        /// Loads Favorites from local storage
        /// </summary>
        /// <returns>Collection of saved Favorites</returns>
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

        #region event handlers

        private void ProgressReporter_ProgressChanged(object sender, DownloadProgressArgs args)
        {
            this.Progress = (int) args.PercentComplete;
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
