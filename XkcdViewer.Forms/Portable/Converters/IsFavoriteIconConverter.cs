using System;
using System.Globalization;
using Xamarin.Forms;

namespace Portable.Converters
{
    public class IsFavoriteIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool) value)
            {
               return Device.OnPlatform(
                   iOS: ImageSource.FromFile("ic_favorite_remove.png"),
                   Android: ImageSource.FromFile("ic_favorite_remove.png"),
                   WinPhone: ImageSource.FromFile("ic_favorite_remove.png"));
            }

            return Device.OnPlatform(
                   iOS: ImageSource.FromFile("ic_favorite_add.png"),
                   Android: ImageSource.FromFile("ic_favorite_add.png"),
                   WinPhone: ImageSource.FromFile("ic_favorite_add.png"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
