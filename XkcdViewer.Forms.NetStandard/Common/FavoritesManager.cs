using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using MonkeyCache.FileStore;
using Newtonsoft.Json;
using XkcdViewer.Forms.NetStandard.Models;

namespace XkcdViewer.Forms.NetStandard.Common
{
    public class FavoritesManager
    {
        private static FavoritesManager _current;
        private readonly ObservableCollection<Comic> favorites;

        public static FavoritesManager Current => _current ?? (_current = new FavoritesManager());

        public FavoritesManager()
        {
            favorites = LoadFavorites();
        }

        public ObservableCollection<Comic> Favorites => favorites;

        public bool IsFavorite(Comic comic)
        {
            return Favorites.Contains(comic);
        }
        
        public void AddFavorite(Comic comic, bool save = true)
        {
            try
            {
                Favorites.Add(comic);

                if(save) 
                    SaveFavorites();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AddFavoriteAsync Exception: {ex}");
            }
        }

        public void RemoveFavorite(Comic comic, bool save = true)
        {
            try
            {
                Favorites.Remove(comic);

                if(save) 
                    SaveFavorites();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RemoveFavoriteAsync Exception: {ex}");
            }
        }

        private ObservableCollection<Comic> LoadFavorites()
        {
            try
            {
                Debug.WriteLine($"---LoadFavoritesAsync called----");

                var favsAsJson = Barrel.Current.Get<string>("serialized_favs");
                
                var favsCollection = JsonConvert.DeserializeObject<ObservableCollection<Comic>>(favsAsJson);
                
                Debug.WriteLine($"---LoadFavoritesAsync: {favsCollection.Count} favorites loaded");

                return favsCollection;
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"LoadFavoritesAsync JSONException: {ex}");
                 
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadFavoritesAsync Exception: {ex}");
            }

            return new ObservableCollection<Comic>();
        }

        public bool SaveFavorites()
        {
            try
            {
                var favsAsJson = JsonConvert.SerializeObject(Favorites);

                Barrel.Current.Add("serialized_favs", favsAsJson, TimeSpan.FromDays(360));

                Debug.WriteLine($"---SaveFavoritesAsync: {Favorites.Count}");

                return true;
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"SaveCollectionAsync JSONException: {ex}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SaveCollectionAsync Exception: {ex}");
                return false;
            }
        }
    }
}
