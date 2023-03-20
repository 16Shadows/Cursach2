using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DMOrganizerApp.UserControls
{
    /// <summary>
    /// Interaction logic for FormattableTextBox.xaml
    /// </summary>
    public partial class FormattableRichTextBox : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(nameof(Document), typeof(FlowDocument), typeof(FormattableRichTextBox), new PropertyMetadata(null, DocumentPropertyChanged));
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(FormattableRichTextBox), new PropertyMetadata(false));

        public event PropertyChangedEventHandler? PropertyChanged;
        private void InvokePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public FlowDocument Document
        {
            get => (FlowDocument)GetValue(DocumentProperty);
            set => SetValue(DocumentProperty, value);
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public List<double> FontSizes { get; }
        public double? CurrentSelectionFontSize
        {
            get => RichTextBox.Selection.GetPropertyValue(TextElement.FontSizeProperty) is double size ? size : null;
            set => RichTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, value ?? 10);
        }

        public IEnumerable<FontFamily> FontFamilies { get; }
        public FontFamily CurrentSelectionFontFamily
        {
            get => RichTextBox.Selection.GetPropertyValue(TextElement.FontFamilyProperty) is FontFamily family ? family : null;
            set => RichTextBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, value);
        }

        public bool IsSelectionBold
        {
            get => FontWeights.Bold.Equals(RichTextBox.Selection.GetPropertyValue(TextElement.FontWeightProperty));
            set => RichTextBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, value ? FontWeights.Bold : FontWeights.Normal);
        }

        public bool IsSelectionItalicised
        {
            get => FontStyles.Italic.Equals(RichTextBox.Selection.GetPropertyValue(TextElement.FontStyleProperty));
            set => RichTextBox.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, value ? FontStyles.Italic : FontStyles.Normal);
        }

        public bool IsSelectionUnderlined
        {
            get => TextDecorations.Underline.Equals(RichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty));
            set => RichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, value ? TextDecorations.Underline : null);
        }

        public FormattableRichTextBox()
        {
            FontSizes = new List<double>();
            for (int i = 1; i <= 40; i++)
                FontSizes.Add(i*2);
            FontFamilies = Fonts.SystemFontFamilies;

            InitializeComponent();
        }

        private void RichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            InvokePropertyChanged(nameof(CurrentSelectionFontFamily));
            InvokePropertyChanged(nameof(CurrentSelectionFontSize));
            InvokePropertyChanged(nameof(IsSelectionBold));
            InvokePropertyChanged(nameof(IsSelectionItalicised));
            InvokePropertyChanged(nameof(IsSelectionUnderlined));
        }

        private static void DocumentPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                ((FormattableRichTextBox)obj).RichTextBox.Document = (FlowDocument)e.NewValue;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Document ??= new FlowDocument();
        }
    }
}
