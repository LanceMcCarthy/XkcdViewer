using XkcdViewer.Maui.Models;
using XkcdViewer.Maui.ViewModels;
using XkcdViewer.Maui.Views;

namespace XkcdViewer.Maui.Services;

//public class ComicSearchHandler : SearchHandler
//{
//    protected override void OnQueryChanged(string oldValue, string newValue)
//    {
//        base.OnQueryChanged(oldValue, newValue);

//        if (string.IsNullOrWhiteSpace(newValue))
//        {
//            ItemsSource = null;
//            return;
//        }

//        Operate(newValue);
//    }

//    protected override async void OnItemSelected(object item)
//    {
//        base.OnItemSelected(item);

//        // allow animation
//        await Task.Delay(1000);

//        Operate(item);
//    }

//    private void Operate(object? item)
//    {
//        switch (Shell.Current.CurrentPage)
//        {
//            case MainPage mp:
//                if (mp.BindingContext is MainPageViewModel mainVm)
//                {
//                    if (item is string searchTerm)
//                    {
//                        ItemsSource = mainVm.Comics.Where(c => c.Title.ToLower().Contains(searchTerm.ToLower())).ToList();
//                    }
//                    else
//                    {
//                        mainVm.CurrentComic = (Comic)item;
//                    }
//                }
//                break;
//            case FavoritesPage fp:
//                if (fp.BindingContext is FavoritesPageViewModel favVm)
//                {
//                    if (item is string searchTerm)
//                    {
//                        ItemsSource = favVm.FavoriteComics.Where(c => c.Title.ToLower().Contains(searchTerm.ToLower())).ToList();
//                    }
//                    else
//                    {
//                        favVm.CurrentFavorite = (Comic)item;
//                    }
//                }
//                break;
//        }
//    }
//}
