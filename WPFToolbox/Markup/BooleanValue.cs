using System;
using System.Windows.Markup;

namespace WPFToolbox.Markup
{
    public class ValueTrue : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return true;
        }
    }

    public class ValueFalse : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return false;
        }
    }

    public class BooleanValue : MarkupExtension
    {
        public string Value { get; set; }

        public BooleanValue(string value)
        {
            Value = value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return bool.Parse(Value);
        }
    }
}
