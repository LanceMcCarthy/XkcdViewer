using System.Collections.ObjectModel;
using System.Diagnostics;
using Newtonsoft.Json;
using XkcdViewer.Maui.Models;

namespace XkcdViewer.Maui.Services;

public class FavoritesService
{
    public FavoritesService()
    {
        Favorites = LoadFavorites();
    }

    public ObservableCollection<Comic?> Favorites { get; }

    public bool IsFavorite(Comic comic)
    {
        return Favorites.Any(c => c?.Num == comic.Num);
    }
        
    public void AddFavorite(Comic? comic, bool save = true)
    {
        try
        {
            Favorites.Add(comic);

            if (comic != null) 
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

    public void RemoveFavorite(Comic? comic, bool save = true)
    {
        try
        {
            Favorites.Remove(comic);

            if (comic != null) 
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

    private ObservableCollection<Comic?> LoadFavorites()
    {
        try
        {
            var json = File.ReadAllText(Path.Combine(FileSystem.AppDataDirectory, "favs.json"));
                
            var favorites = JsonConvert.DeserializeObject<ObservableCollection<Comic>>(json);

            // Provides backwards support for comics that were saved before the IsFavorite property was available.
            if (favorites != null)
            {
                foreach (var comic in favorites)
                {
                    if (!comic.IsFavorite) comic.IsFavorite = true;
                }

                Debug.WriteLine($"---LoadFavoritesAsync: {favorites.Count} favorites loaded");

                return favorites!;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"LoadFavoritesAsync Exception: {ex}");
        }

        return new ObservableCollection<Comic?>();
    }

    public bool SaveFavorites()
    {
        try
        {
            var json = JsonConvert.SerializeObject(Favorites);

            var filePath = Path.Combine(FileSystem.AppDataDirectory, "favs.json");

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