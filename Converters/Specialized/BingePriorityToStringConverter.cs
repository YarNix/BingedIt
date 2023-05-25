using System;
using System.Globalization;
using System.Windows.Data;
using BingedIt.Common;

namespace BingedIt.Converters.Specialized
{
    [ValueConversion(typeof(BingePriority), typeof(string))]
    internal class BingePriorityToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is BingePriority priority ? priority.AsString() : null!;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
