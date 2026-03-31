using System.Globalization;
using XkcdViewer.Maui.Models;

namespace XkcdViewer.Maui.Converters;

public class ComicToImageSourceConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Comic comic) 
            return null!;

        return comic.Title == "Garden" || string.IsNullOrEmpty(comic.Img)
            ? ImageSource.FromFile("garden_256.png")
            : ImageSource.FromUri(new Uri(comic.Img));
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}