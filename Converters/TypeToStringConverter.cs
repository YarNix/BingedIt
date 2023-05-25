using System;
using System.Globalization;
using System.Windows.Data;

namespace BingedIt.Converters
{
    [ValueConversion(typeof(object), typeof(string))]
    public class TypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is null ? "Null" : value.GetType().ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
