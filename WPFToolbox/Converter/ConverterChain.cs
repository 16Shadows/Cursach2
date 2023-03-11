using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace WPFToolbox.Converter
{
    /// <summary>
    /// Executes multiple converters in a sequential order, providing result of a previous conversion to a following converter
    /// </summary>
    public class ConverterChain : List<IValueConverter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (IValueConverter converter in this)
                value = converter.Convert(value, targetType, parameter, culture);
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (IValueConverter converter in ((IEnumerable<IValueConverter>)this).Reverse())
                value = converter.ConvertBack(value, targetType, parameter, culture);
            return value;
        }
    }
}
