using System;
using System.Globalization;
using Xamarin.Forms;
using XkcdViewer.Forms.NetStandard.Models;

namespace XkcdViewer.Forms.NetStandard.Converters
{
    public class ComicToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Comic comic)
            {
                if (comic.Title == "Garden" || string.IsNullOrEmpty(comic.Img))
                {
                    return ImageSource.FromFile("garden_256.png");
                }
                else
                {
                    return ImageSource.FromUri(new Uri(comic.Img));
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
