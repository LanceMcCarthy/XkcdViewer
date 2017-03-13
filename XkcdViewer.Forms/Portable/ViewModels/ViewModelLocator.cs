/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Portable"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Portable.Common;

namespace Portable.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<NavigationService>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<FavoritesPageViewModel>();
            SimpleIoc.Default.Register<DetailsPageViewModel>();
        }
        
        public static MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public static FavoritesPageViewModel Favorites => ServiceLocator.Current.GetInstance<FavoritesPageViewModel>();

        public static DetailsPageViewModel Details => ServiceLocator.Current.GetInstance<DetailsPageViewModel>();

        public static bool IsDesignTime { get; } = Xamarin.Forms.Application.Current == null;
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
