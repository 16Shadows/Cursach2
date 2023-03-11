using System.Globalization;
using System.Windows.Controls;

namespace WPFToolbox.Validation
{
    public sealed class DoubleValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string s = (string)value;
            if (!double.TryParse(s, NumberStyles.Float, cultureInfo, out _))
                return new ValidationResult(false, "Not a number");
            return ValidationResult.ValidResult;
        }
    }
}
