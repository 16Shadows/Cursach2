using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace WPFToolbox.Converter
{
    public class NullToLoading : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return value;
            if (targetType.IsSubclassOf(typeof(IEnumerable)) || targetType == typeof(IEnumerable))
                return new string[] { Resources.CommonLocale.LocaleKey_Loading };
            else
                return Resources.CommonLocale.LocaleKey_Loading;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
