using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BingedIt.Converters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class IsValueSetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || value == DependencyProperty.UnsetValue)
                return false;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
