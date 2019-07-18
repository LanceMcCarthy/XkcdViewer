using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using XkcdViewer.Forms.Models;

namespace XkcdViewer.Forms.Common
{
    public class FavoritesManager
    {
        private static FavoritesManager _current;

        public static FavoritesManager Current => _current ?? (_current = new FavoritesManager());

        public FavoritesManager()
        {
            Favorites = LoadFavorites();
        }

        public ObservableCollection<Comic> Favorites { get; }

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
                var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var json = File.ReadAllText(Path.Combine(localFolder, "favs.json"));
                
                var favorites = JsonConvert.DeserializeObject<ObservableCollection<Comic>>(json);
                
                Debug.WriteLine($"---LoadFavoritesAsync: {favorites.Count} favorites loaded");

                return favorites;
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
                var json = JsonConvert.SerializeObject(Favorites);

                var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var filePath = Path.Combine(localFolder, "favs.json");

                if (File.Exists(filePath))
                    File.Delete(filePath);

                File.WriteAllText(filePath, json);
                
                Debug.WriteLine($"---SaveFavoritesAsync: {Favorites.Count}");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SaveCollectionAsync Exception: {ex}");
                return false;
            }
        }
    }
}