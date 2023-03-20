using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;

namespace DMOrganizerApp.Converter
{
    public class RtfToFlowDocument : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            string rtf = (string)value;
            if (rtf.Length == 0)
                return new FlowDocument();
            using MemoryStream stream = new ();
            using StreamWriter writer = new (stream);
            writer.Write(rtf);
            writer.Flush();
            FlowDocument doc = new ();
            TextRange range = new (doc.ContentStart, doc.ContentEnd);
            stream.Seek(0, SeekOrigin.Begin);
            try
            {
                range.Load(stream, DataFormats.Rtf);
            }
            catch(Exception)
            {
                MessageBox.Show(rtf);
            }
            return doc;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            FlowDocument doc = (FlowDocument)value;
            TextRange range = new (doc.ContentStart, doc.ContentEnd);
            using MemoryStream stream = new ();
            range.Save(stream, DataFormats.Rtf);
            stream.Seek(0, SeekOrigin.Begin);
            using StreamReader reader = new (stream);
            return reader.ReadToEnd();
        }
    }
}
