using System;
using System.Globalization;
using Xamarin.Forms;

namespace XkcdViewer.Forms.Converters
{
    public class IsFavoriteIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool) value)
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        return ImageSource.FromFile("ic_favorite_remove.png");
                    case Device.Android:
                        return ImageSource.FromFile("ic_favorite_remove.png");
                    case Device.UWP:
                        return ImageSource.FromFile("ic_favorite_remove.png");
                    default:
                        return ImageSource.FromFile("ic_favorite_remove.png");
                }
            }

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    return ImageSource.FromFile("ic_favorite_add.png");
                case Device.Android:
                    return ImageSource.FromFile("ic_favorite_add.png");
                case Device.UWP:
                    return ImageSource.FromFile("ic_favorite_add.png");
                default:
                    return ImageSource.FromFile("ic_favorite_add.png");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
