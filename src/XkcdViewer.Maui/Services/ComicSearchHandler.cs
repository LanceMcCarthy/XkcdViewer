using XkcdViewer.Maui.Models;
using XkcdViewer.Maui.ViewModels;
using XkcdViewer.Maui.Views;

namespace XkcdViewer.Maui.Services;

public class ComicSearchHandler : SearchHandler
{
    protected override void OnQueryChanged(string oldValue, string newValue)
    {
        base.OnQueryChanged(oldValue, newValue);

        if (string.IsNullOrWhiteSpace(newValue))
        {
            ItemsSource = null;
            return;
        }

        switch (Shell.Current.CurrentPage)
        {
            case MainPage mp:
                ItemsSource = (mp.BindingContext as MainPageViewModel)?.Comics
                    .Where(c => c.Title.ToLower().Contains(newValue.ToLower()))
                    .ToList();
                break;
            case FavoritesPage fp:
                ItemsSource = (fp.BindingContext as FavoritesPageViewModel)?.FavoriteComics
                    .Where(c => c.Title.ToLower().Contains(newValue.ToLower()))
                    .ToList();
                break;
        }
    }

    protected override async void OnItemSelected(object item)
    {
        base.OnItemSelected(item);

        // allow animation
        await Task.Delay(1000);

        // Set the SelectedItem of that page's SlideView
        switch (Shell.Current.CurrentPage)
        {
            case MainPage mp:
                if(mp.BindingContext is MainPageViewModel mainVm)
                    mainVm.CurrentComic = (Comic)item;
                break;
            case FavoritesPage fp:
                if(fp.BindingContext is FavoritesPageViewModel favVm)
                    favVm.CurrentFavorite = (Comic)item;
                break;
        }
    }
}
