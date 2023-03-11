using System.Globalization;
using System.Windows.Controls;

namespace WPFToolbox.Validation
{
    public sealed class IntegerValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string s = (string)value;
            if (!int.TryParse(s, NumberStyles.Integer, cultureInfo, out _))
                return new ValidationResult(false, "Not an integer");
            return ValidationResult.ValidResult;
        }
    }
}
