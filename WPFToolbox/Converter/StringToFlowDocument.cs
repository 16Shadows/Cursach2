using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Documents;

namespace WPFToolbox.Converter
{
    public class StringToFlowDocument : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new FlowDocument(new Paragraph(new Run(value as string)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FlowDocument doc = (FlowDocument)value;
            return new TextRange(doc.ContentStart, doc.ContentEnd).Text;
        }
    }
}
