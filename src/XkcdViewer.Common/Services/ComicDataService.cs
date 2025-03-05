using CommonHelpers.Services;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using XkcdViewer.Common.Models;

namespace XkcdViewer.Common.Services;

public class ComicDataService(XkcdApiService apiServ)
{
    private readonly string dataFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "appdata.json");

    public async Task GetComic(ObservableCollection<Comic>? comics, int? comicNumber)
    {
        if (comics == null)
            throw new NullReferenceException("You must pass a valid reference to the source comics collection.");

        Comic comic;

        if (comicNumber == null)
        {
            var result = await apiServ.GetNewestComicAsync();
            comic = result.ToComic();
        }
        else
        {
            var result = await apiServ.GetComicAsync(comicNumber.Value);
            comic = result.ToComic();
        }

        comics.Add(comic);

        await SaveComicsAsync(comics);
    }

    public async Task LoadComicsAsync(ObservableCollection<Comic>? comics)
    {
        if (comics == null)
            throw new NullReferenceException("You must pass a valid reference to the source comics collection.");

        ObservableCollection<Comic>? deserializedComics = null;

        try
        {
            var jsonString = await File.ReadAllTextAsync(dataFilePath);
            deserializedComics = JsonConvert.DeserializeObject<ObservableCollection<Comic>>(jsonString);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"LoadComics Exception: {ex}");
        }

        if (deserializedComics == null)
        {
            var result = await apiServ.GetNewestComicAsync();
            comics.Add(result.ToComic());

            await SaveComicsAsync(comics);
        }
        else
        {
            if(comics.Count > 0)
                comics.Clear();

            foreach (var comic in deserializedComics)
            {
                comics.Add(comic);
            }
        }
    }

    public async Task LoadFavoriteComicsAsync(ObservableCollection<Comic>? comics)
    {
        if (comics == null)
            throw new NullReferenceException("You must pass a valid reference to the source comics collection.");

        ObservableCollection<Comic>? deserializedComics = null;

        try
        {
            var jsonString = await File.ReadAllTextAsync(dataFilePath);
            deserializedComics = JsonConvert.DeserializeObject<ObservableCollection<Comic>>(jsonString);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"LoadComics Exception: {ex}");
        }

        if (deserializedComics != null)
        {
            if(comics.Count > 0)
                comics.Clear();

            var favoriteComics = deserializedComics.Where(c => c.IsFavorite);

            foreach (var comic in favoriteComics)
            {
                comics.Add(comic);
            }
        }
    }

    public async Task<bool> SaveComicsAsync(ObservableCollection<Comic>? comics)
    {
        if (comics == null)
            throw new NullReferenceException("You must pass a valid reference to the source comics collection.");

        try
        {
            var json = JsonConvert.SerializeObject(comics);

            if (File.Exists(dataFilePath))
                File.Delete(dataFilePath);

            await File.WriteAllTextAsync(dataFilePath, json);
                
            Debug.WriteLine($"-****- SaveComics: {comics.Count} -****-");

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"--- SaveComics Exception: {ex}");
            return false;
        }
    }
}