using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace BingedIt.Converters.Specialized
{
    internal class StringFirstLetterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? stringVal = value as string;
            if (stringVal is null)
            {
                MethodInfo toStringMethod = value.GetType().GetMethod(nameof(ToString), BindingFlags.Instance | BindingFlags.Public, Type.EmptyTypes)!;
                if (toStringMethod.DeclaringType != typeof(object))
                    // The object implement its own ToString()
                    stringVal = value.ToString();
            }
            char? firstLetter = stringVal?.FirstOrDefault()!;
            if (firstLetter is char c && char.IsAsciiLetter(c))
                return char.ToUpper(c);
            return "Others";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
