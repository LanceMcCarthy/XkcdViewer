using CommonHelpers.Services.DataModels;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using XkcdViewer.Maui.Common;

namespace XkcdViewer.Maui.Models;

public class Comic : XkcdComic, INotifyPropertyChanged
{
    private bool isFavorite;

    public Comic()
    {
        ShareCommand = new Command(async () => { await Share(); });
        ToggleFavoriteCommand = new Command(ToggleIsFavorite);
    }
        
    public bool IsFavorite
    {
        get => isFavorite;
        set => UpdateValue(ref isFavorite, value);
    }

    [JsonIgnore]
    public Command ShareCommand { get; }

    [JsonIgnore]
    public Command ToggleFavoriteCommand { get; }

    public async Task Share()
    {
        Debug.WriteLine($"Share Comic: {Title}");

        if (string.IsNullOrEmpty(Img))
            return;
            
        await Microsoft.Maui.ApplicationModel.DataTransfer.Share.Default.RequestAsync(
            new ShareTextRequest
            {
                Title = Title ?? "xkcd",
                Text = Transcript ?? "",
                Uri = Img
            });
    }

    private void ToggleIsFavorite()
    {
        if (IsFavorite)
        {
            FavoritesManager.Current.RemoveFavorite(this);
        }
        else
        {
            FavoritesManager.Current.AddFavorite(this);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool UpdateValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}