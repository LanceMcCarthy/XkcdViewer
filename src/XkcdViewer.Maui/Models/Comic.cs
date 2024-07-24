using CommonHelpers.Services.DataModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XkcdViewer.Maui.Models;

public class Comic : XkcdComic, INotifyPropertyChanged
{
    private bool isFavorite; 
    public bool IsFavorite
    {
        get => isFavorite;
        set => UpdateValue(ref isFavorite, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

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