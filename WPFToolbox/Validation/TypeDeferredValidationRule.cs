using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPFToolbox.Validation
{
    public class TypeDeferredValidationRule : ValidationRule
    {
        protected Type? m_ValidationRuleType;
        public Type? ValidationRuleType
        {
            get => m_ValidationRuleType;
            set
            {
                RuleInstance = null;
                m_ValidationRuleType = value;
                if (value == null)
                    return;

                if (!value.IsSubclassOf(typeof(ValidationRule)))
                    throw new ArgumentException("Invalid type, not a validation rule", nameof(ValidationRuleType));
                RuleInstance = (ValidationRule?)Activator.CreateInstance(value);
                if (RuleInstance == null)
                    throw new ArgumentException("Failed to create instance of provided type", nameof(ValidationRuleType));
            }
        }
        protected ValidationRule? RuleInstance { get; private set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return RuleInstance?.Validate(value, cultureInfo) ?? ValidationResult.ValidResult;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo, BindingExpressionBase owner)
        {
            return RuleInstance?.Validate(value, cultureInfo, owner) ?? ValidationResult.ValidResult;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo, BindingGroup owner)
        {
            return RuleInstance?.Validate(value, cultureInfo, owner) ?? ValidationResult.ValidResult;
        }
    }
}
