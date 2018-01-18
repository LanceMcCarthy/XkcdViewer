using System;
using System.Globalization;
using Xamarin.Forms;

namespace XkcdViewer.Forms.NetStandard.Converters
{
    class IsFavoriteToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value ? "Unfavorite" : "Favorite";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == "Unfavorite";
        }
    }
}
