using System;
using System.Windows.Markup;

namespace WPFToolbox.Markup
{
    public class IntegerValue : MarkupExtension
    {
        public string Value { get; set; }

        public IntegerValue(string value)
        {
            Value = value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return int.Parse(Value);
        }
    }
}
