using System.Globalization;

namespace XkcdViewer.Maui.Converters;

public sealed class ConfigurableBoolConverter<T> : IValueConverter
{
    public ConfigurableBoolConverter() { }

    public ConfigurableBoolConverter(T trueResult, T falseResult)
    {
        TrueResult = trueResult;
        FalseResult = falseResult;
    }

    public T? TrueResult { get; set; }

    public T? FalseResult { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return TrueResult == null || FalseResult == null 
            ? !(bool)value! 
            : value is true 
                ? TrueResult 
                : FalseResult;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return TrueResult == null || FalseResult == null
            ? !(bool)value!
            : value is T variable && EqualityComparer<T>.Default.Equals(variable, TrueResult);
    }
}