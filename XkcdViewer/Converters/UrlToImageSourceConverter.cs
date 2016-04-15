using System;
using System.Globalization;
using Xamarin.Forms;

namespace XkcdViewer.Converters
{
    public class UrlToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty((string) value))
            {
                return ImageSource.FromFile("Images/MrSadSideburns.png");
            }
            else
            {
                return ImageSource.FromUri(new Uri((string)value));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
