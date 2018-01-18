using System;
using System.Globalization;
using Xamarin.Forms;

namespace XkcdViewer.Forms.NetStandard.Converters
{
    public class IsGardenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            return (string)value != "Garden";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
