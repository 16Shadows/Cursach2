using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFToolbox.Converter
{
    /// <summary>
    /// Instantiates a converter of the provided type and defers conversion to it.
    /// Intended to be used in conjunction with markup extension NestedType to use converters which are declared as nested types
    /// </summary>
    internal class TypeDeferredConverter : IValueConverter
    {
        protected Type? m_ConverterType;
        /// <summary>
        /// The type of the converter to use. May be null, in which case no conversion is performed
        /// </summary>
        /// <exception cref="ArgumentException">Is thrown either when the value is not a IValueConverter or the Type does not provide a public parameterless constructor</exception>
        public Type? ConverterType
        {
            get => m_ConverterType;
            set
            {
                ConverterInstance = null;
                m_ConverterType = value;
                if (value == null)
                    return;

                if (!value.IsAssignableTo(typeof(IValueConverter)))
                    throw new ArgumentException("Invalid type, not a value converter", nameof(ConverterType));
                ConverterInstance = (IValueConverter?)Activator.CreateInstance(value);
                if (ConverterInstance == null)
                    throw new ArgumentException("Failed to create instance of provided type", nameof(ConverterType));
            }
        }
        protected IValueConverter? ConverterInstance { get; set; }

        public TypeDeferredConverter() { }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConverterInstance != null ? ConverterInstance?.Convert(value, targetType, parameter, culture) : value;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConverterInstance != null ? ConverterInstance.ConvertBack(value, targetType, parameter, culture) : value;
        }
    }
}
