using System;
using System.Windows.Markup;

namespace WPFToolbox.Markup
{
    public class DoubleValue : MarkupExtension
    {
        public string Value { get; set; }

        public DoubleValue(string value)
        {
            Value = value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return double.Parse(Value);
        }
    }
}
