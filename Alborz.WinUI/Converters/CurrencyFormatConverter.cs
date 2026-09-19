using Microsoft.UI.Xaml.Data;
using System;

namespace Alborz.WinUI.Converters;

public class CurrencyFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is decimal decimalValue)
        {
            return decimalValue.ToString("N0");
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return decimal.TryParse(value?.ToString(), System.Globalization.NumberStyles.Number,
            System.Globalization.CultureInfo.CurrentCulture, out var amount)
            ? amount : Microsoft.UI.Xaml.DependencyProperty.UnsetValue;
    }
}

