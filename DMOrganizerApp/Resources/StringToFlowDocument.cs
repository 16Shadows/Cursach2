using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Documents;

namespace DMOrganizerApp.Resources
{
    internal class StringToFlowDocument : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new FlowDocument(new Paragraph(new Run(value as string)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
