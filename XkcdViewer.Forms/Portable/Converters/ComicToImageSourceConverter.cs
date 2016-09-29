using System;
using System.Globalization;
using Portable.Models;
using Xamarin.Forms;

namespace Portable.Converters
{
    public class ComicToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var comic = value as Comic;

            if (comic == null)
                return null;

            if (comic.Title == "Garden" || string.IsNullOrEmpty(comic.Img))
            {
                return ImageSource.FromFile("garden_256.png");
            }
            else
            {
                return ImageSource.FromUri(new Uri(comic.Img.Insert(4, "s")));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
