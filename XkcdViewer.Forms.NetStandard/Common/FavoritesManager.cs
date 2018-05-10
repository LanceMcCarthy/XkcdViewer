using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using XkcdViewer.Forms.NetStandard.Models;

namespace XkcdViewer.Forms.NetStandard.Common
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
                Debug.WriteLine($"---LoadFavoritesAsync called----");

                var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                var favsAsJson = File.ReadAllText(Path.Combine(localFolder, "favs.json"));
                
                var favsCollection = JsonSerializer<ObservableCollection<Comic>>.DeSerialize(favsAsJson);
                //var favsCollection = JsonConvert.DeserializeObject<ObservableCollection<Comic>>(favsAsJson);
                
                Debug.WriteLine($"---LoadFavoritesAsync: {favsCollection.Count} favorites loaded");

                return favsCollection;
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
                var favsAsJson = JsonSerializer<ObservableCollection<Comic>>.Serialize(Favorites);
                //var favsAsJson = JsonConvert.SerializeObject(Favorites);

                var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                var filePath = Path.Combine(localFolder, "favs.json");

                if (File.Exists(filePath))
                    File.Delete(filePath);

                File.WriteAllText(filePath, favsAsJson);
                
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