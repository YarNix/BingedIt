using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BingedIt.Converters.Specialized
{
    internal class TransparentFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != Brushes.Transparent)
                return value;
            return null!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
