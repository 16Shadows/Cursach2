using System;
using System.Globalization;
using System.Windows.Data;
using WPFToolbox.Condition;

namespace WPFToolbox.Converter
{
    /// <summary>
    /// Converts a result of ICondition to a boolean value. Ignores value provided by the binding
    /// </summary>
    public class ConditionResultConverter : IValueConverter
    {
        public ICondition? Condition { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Condition?.Evaluate() ?? false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
