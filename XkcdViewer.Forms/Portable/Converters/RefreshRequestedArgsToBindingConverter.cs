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

            // Effectively the same as calling ListView.EndRefresh
            if(eventArgs!=null) eventArgs.Cancel = true;
            
            return eventArgs;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
