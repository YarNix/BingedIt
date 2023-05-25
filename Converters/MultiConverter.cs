using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace BingedIt.Converters
{
    public sealed class ValueConverterCollection : Collection<IValueConverter> { }
    /// <summary>
    /// Allow to creating new converter by combining existing ones
    /// </summary>
    [ContentProperty("Converters")]
    [ContentWrapper(typeof(ValueConverterCollection))]
    public class MultiConverter : IValueConverter
    {
        private readonly ValueConverterCollection _converters = new();

        public ValueConverterCollection Converters => _converters;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IValueConverter converter = null!;
            try
            {
                for (int i = 0; i < _converters.Count; i++)
                {
                    converter = _converters[i];
                    value = converter.Convert(value, targetType, parameter, culture);
                }
            }
            catch (NotImplementedException)
            {
                throw new NotImplementedException($"{converter.GetType()}.Convert() threw {nameof(NotImplementedException)}");
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IValueConverter converter = null!;
            try
            {
                for (int i = 0; i < _converters.Count; i++)
                {
                    converter = _converters[i];
                    value = converter.ConvertBack(value, targetType, parameter, culture);
                }
            }
            catch (NotImplementedException)
            {
                throw new NotImplementedException($"{converter.GetType()}.Convert() threw {nameof(NotImplementedException)}");
            }
            return value;
        }
    }
}
