using System;
using System.ComponentModel;
using System.Windows.Data;

namespace BingedIt.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    sealed class SortingAttribute : Attribute
    {
        Type? _valueConverterType;
        public SortingAttribute(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));
            PropertyName = propertyName;
        }
        public string PropertyName { get; }
        public ListSortDirection DefaultDirection { get; set; } = ListSortDirection.Ascending;

        public Type? ValueConverterType
        {
            get => _valueConverterType;
            set
            {
                if (_valueConverterType is not null && _valueConverterType.IsClass && _valueConverterType.GetInterface(nameof(IValueConverter)) is not null)
                    throw new ArgumentException($"Type must implement {nameof(IValueConverter)}", nameof(value));
                _valueConverterType = value;
            }
        }
        public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;

        public SortDescription GetSortDescription() => new SortDescription(PropertyName, DefaultDirection);
        public GroupDescription GetGroupDescription()
        {
            if (_valueConverterType is null)
                return new PropertyGroupDescription(PropertyName);
            IValueConverter valueConverter = (IValueConverter)Activator.CreateInstance(_valueConverterType)!;
            return new PropertyGroupDescription(PropertyName, valueConverter, StringComparison);
        }
    }
}
