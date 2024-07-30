using CommonHelpers.Services;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using XkcdViewer.Maui.Models;

namespace XkcdViewer.Maui.Services;

public class DataService(XkcdApiService apiServ)
{
    private readonly string dataFilePath = Path.Combine(FileSystem.AppDataDirectory, "appdata.json");

    public ObservableCollection<Comic>? Comics { get; set; }

    public async Task GetComic(int comicNumber)
    {
        var result = await apiServ.GetComicAsync(comicNumber);

        Comics.Add(result.ToComic());

        await SaveComicsAsync();
    }

    public async Task LoadNewComic()
    {
        var oldestId = Comics.LastOrDefault()?.Num;

        if(oldestId == null)
            return;

        await GetComic(oldestId.Value - 1);
    }

    public async Task LoadComicsAsync()
    {
        try
        {
            var json = await File.ReadAllTextAsync(dataFilePath);
            Comics = JsonConvert.DeserializeObject<ObservableCollection<Comic>>(json);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"LoadComics Exception: {ex}");

            // If the file was missing, it is the user's first time running the app

            // 1. New up the collection
            Comics = new ObservableCollection<Comic>();

            // 2. Get the most recent comic
            var result = await apiServ.GetNewestComicAsync();
            Comics.Add(result.ToComic());

            // 3. Save the list
            await SaveComicsAsync();
        }
    }

    public async Task<bool> SaveComicsAsync()
    {
        try
        {
            var json = JsonConvert.SerializeObject(Comics);

            if (File.Exists(dataFilePath))
                File.Delete(dataFilePath);

            await File.WriteAllTextAsync(dataFilePath, json);
                
            Debug.WriteLine($"-****- SaveComics: {Comics.Count} -****-");

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"--- SaveComics Exception: {ex}");
            return false;
        }
    }
}