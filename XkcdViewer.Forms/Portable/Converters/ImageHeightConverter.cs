using System;
using System.Globalization;
using Portable.Models;
using Xamarin.Forms;

namespace Portable.Converters
{
    public class ImageHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0;

            return ((Comic) value).Title == "Garden" ? 0 : 300;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
