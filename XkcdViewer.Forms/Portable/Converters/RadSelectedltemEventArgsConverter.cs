using System;
using System.Diagnostics;
using System.Globalization;
using Telerik.XamarinForms.DataControls.ListView;
using Xamarin.Forms;

namespace Portable.Converters
{
    public class RadSelectedltemEventArgsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var eventArgs = value as ItemTapEventArgs;
            Debug.WriteLine($"RadSelectedltemEventArgsConverter fired - EventArgs Item: {eventArgs?.Item}");
            return eventArgs?.Item;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
