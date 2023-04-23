using System.Collections.ObjectModel;
using System.Diagnostics;
using Newtonsoft.Json;
using XkcdViewer.Maui.Models;

namespace XkcdViewer.Maui.Common;

public class FavoritesManager
{
    private static FavoritesManager _current;

    public static FavoritesManager Current => _current ??= new FavoritesManager();

    public FavoritesManager()
    {
        Favorites = LoadFavorites();
    }

    public ObservableCollection<Comic> Favorites { get; }

    public bool IsFavorite(Comic comic)
    {
        return Favorites.Any(c => c.Num == comic.Num);
    }
        
    public void AddFavorite(Comic comic, bool save = true)
    {
        try
        {
            Favorites.Add(comic);

            comic.IsFavorite = true;

            if(save)
            {
                SaveFavorites();
            }
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

            comic.IsFavorite = false;

            if(save)
            {
                SaveFavorites();
            }
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

            // Provides backwards support for comics that were saved before the IsFavorite property was available.
            foreach (var comic in favorites)
            {
                if (!comic.IsFavorite) comic.IsFavorite = true;
            }
                
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