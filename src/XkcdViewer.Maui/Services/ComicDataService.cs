using CommonHelpers.Services;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using XkcdViewer.Maui.Models;

namespace XkcdViewer.Maui.Services;

public class ComicDataService(XkcdApiService apiServ)
{
    private readonly string dataFilePath = Path.Combine(FileSystem.AppDataDirectory, "appdata.json");

    public async Task GetComic(ObservableCollection<Comic> comics, int? comicNumber)
    {
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

    public async Task LoadComicsAsync(ObservableCollection<Comic> comics)
    {
        string jsonString = string.Empty;
        ObservableCollection<Comic>? deserializedComics = null;

        try
        {
            jsonString = await File.ReadAllTextAsync(dataFilePath);
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
            foreach (var comic in deserializedComics)
            {
                comics.Add(comic);
            }
        }
    }

    public async Task<bool> SaveComicsAsync(ObservableCollection<Comic> comics)
    {
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