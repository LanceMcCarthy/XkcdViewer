using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using MonkeyCache.FileStore;
using Newtonsoft.Json;
using XkcdViewer.Forms.NetStandard.Models;

namespace XkcdViewer.Forms.NetStandard.Common
{
    public static class FileHelpers
    {
        public static bool SaveCollection<T>(IEnumerable<T> items, string fileName)//
        {
            try
            {
                var favsAsJson = JsonConvert.SerializeObject(items);

                Barrel.Current.Add("serialized_favs", favsAsJson, TimeSpan.MaxValue);
                
                Debug.WriteLine($"---SaveCollectionAsync: {items.Count()} items saved to {fileName}");

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

        public static bool SaveFavorites(ObservableCollection<Comic> items)
        {
            try
            {
                var favsAsJson = JsonConvert.SerializeObject(items);

                Barrel.Current.Add("serialized_favs", favsAsJson, TimeSpan.FromDays(360));

                Debug.WriteLine($"---SaveFavoritesAsync: {items.Count}");

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

        public static ObservableCollection<Comic> LoadFavorites()
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
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadFavoritesAsync Exception: {ex}");
                return null;
            }
        }
    }
}
