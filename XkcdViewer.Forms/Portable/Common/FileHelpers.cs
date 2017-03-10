using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PCLStorage;
using Portable.Models;

namespace Portable.Common
{
    public static class FileHelpers
    {
        private static string FavoritesFileName = "FavoritesJsonData.txt";

        public static async Task<bool> SaveCollectionAsync<T>(IEnumerable<T> items, string fileName)//
        {
            try
            {
                var favsAsJson = JsonConvert.SerializeObject(items);
                var file = await FileSystem.Current.LocalStorage.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                await file.WriteAllTextAsync(favsAsJson);

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

        public static async Task<bool> SaveFavoritesAsync(ObservableCollection<Comic> items)
        {
            try
            {
                var favsAsJson = JsonConvert.SerializeObject(items);
                var file = await FileSystem.Current.LocalStorage.CreateFileAsync(FavoritesFileName, CreationCollisionOption.ReplaceExisting);
                await file.WriteAllTextAsync(favsAsJson);

                Debug.WriteLine($"---SaveFavoritesAsync: {items.Count()}");

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

        public static async Task<ObservableCollection<Comic>> LoadFavoritesAsync()
        {
            try
            {
                Debug.WriteLine($"---LoadFavoritesAsync called----");
                
                //using PCLStorage implementaiotn
                var file = await FileSystem.Current.LocalStorage.GetFileAsync(FavoritesFileName);

                var favsAsJson = await file.ReadAllTextAsync();
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
