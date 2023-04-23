using System.Globalization;
using XkcdViewer.Maui.Models;

namespace XkcdViewer.Maui.Converters;

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