using System;
using System.Globalization;
using Xamarin.Forms;
using XkcdViewer.Models;

namespace XkcdViewer.Converters
{
    public class ComicToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var comic = (Comic) value;

            if (comic == null)
                return null;

            if (comic.Title == "Garden" || string.IsNullOrEmpty(comic.Img))
            {
                return ImageSource.FromFile("Images/MrSadSideburns.png");
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
