using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFToolbox.Converter
{
    public class DoubleToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double? dvalue = (double?)value;
            return dvalue.HasValue ? dvalue.Value.ToString() : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return double.Parse((string)value, culture);
        }
    }
}
