using System;
using System.Diagnostics;
using System.Globalization;
using Telerik.XamarinForms.DataControls.ListView;
using Xamarin.Forms;

namespace Portable.Converters
{
    public class RefreshRequestedArgsToBindingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var eventArgs = value as PullToRefreshRequestedEventArgs;
            Debug.WriteLine($"PullToRefreshRequestedEventArgsConverter fired - EventArgs Item: {eventArgs}");
            return eventArgs;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
