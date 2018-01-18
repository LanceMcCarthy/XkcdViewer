using GalaSoft.MvvmLight.Ioc;
using XkcdViewer.Forms.NetStandard.Common;

namespace XkcdViewer.Forms.NetStandard.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            SimpleIoc.Default.Register<NavigationService>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<FavoritesPageViewModel>();
            SimpleIoc.Default.Register<DetailsPageViewModel>();
        }
        
        public static MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();

        public static FavoritesPageViewModel Favorites => SimpleIoc.Default.GetInstance<FavoritesPageViewModel>();

        public static DetailsPageViewModel Details => SimpleIoc.Default.GetInstance<DetailsPageViewModel>();

        public static bool IsDesignTime { get; } = Xamarin.Forms.Application.Current == null;
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
