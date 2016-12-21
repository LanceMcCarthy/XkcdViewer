using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portable.ViewModels
{
    public static class ViewModelLocator
    {
        private static MainViewModel main;
        private static FavoritesPageViewModel favorites;
        private static DetailsPageViewModel details;

        public static MainViewModel Main => main ?? (main = new MainViewModel());

        public static FavoritesPageViewModel Favorites => favorites ?? (favorites = new FavoritesPageViewModel());

        public static DetailsPageViewModel Details => details ?? (details = new DetailsPageViewModel());

        public static bool IsDesignTime { get; } = Xamarin.Forms.Application.Current == null;
    }
}
