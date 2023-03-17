using CSToolbox;
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace DMOrganizerApp.UserControls
{
    /// <summary>
    /// Interaction logic for ExclusiveInputBox.xaml
    /// </summary>
    public partial class ExclusiveInputBox : Window, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private string m_Text;
        public string Text
        {
            get => m_Text;
            set
            {
                if (m_Text == value)
                    return;
                m_Text = value;
                InvokePropertyChanged(nameof(Text));
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Text)));
            }
        }
        private string m_InputPrompt;
        public string InputPrompt
        {
            get => m_InputPrompt;
            set
            {
                if (m_InputPrompt == value)
                    return;
                m_InputPrompt = value;
                InvokePropertyChanged(nameof(InputPrompt));
            }
        }
        public Func<string, CultureInfo, bool>? Validator { get; set; }

        public bool HasErrors => GetErrors(null).Any();
        public IEnumerable GetErrors(string? propertyName)
        {
            if (Validator?.Invoke(Text, CultureInfo.CurrentCulture) == false)
                yield return "Invalid value";

            yield break;
        }

        public ExclusiveInputBox()
        {
            m_Text = "";
            m_InputPrompt = "";
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        protected void InvokePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }   
    }
}
