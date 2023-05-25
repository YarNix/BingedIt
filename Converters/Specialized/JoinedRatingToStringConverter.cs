using System;
using System.Globalization;
using System.Windows.Data;
using BingedIt.Common;

namespace BingedIt.Converters.Specialized
{
    // Split Rating EnumFlag into multiple groups
    internal class JoinedRatingToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            const string flagFormat = "F",
                         flagSeparator = ", ";
            if (value is Rating enumValue)
                // Letting ToString do the heavy lifting
                // ToString: "Flag1, Flag2, Flag3" => Split: { "Flag1", "Flat2", "Flag3" }
                return enumValue.ToString(flagFormat).Split(flagSeparator);
            return null!;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
