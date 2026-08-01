using Microsoft.UI.Xaml.Data;
using System;

namespace Alborz.WinUI.Converters;

public class CurrencyFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is decimal decimalValue)
        {
            // Format: "N0" means numbers with thousands separators and 0 decimal places.
            return decimalValue.ToString("N0");
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
