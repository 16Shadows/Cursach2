using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DMOrganizerApp.Views
{
    /// <summary>
    /// Interaction logic for InputDialogueView.xaml
    /// </summary>
    public sealed partial class InputDialogView : Window, INotifyPropertyChanged
    {
        #region Properties
        private string m_InputText;
        public string InputText
        {
            get => m_InputText;
            set
            {
                m_InputText = value ?? "";
                InvokePropertyChanged(nameof(InputText));
            }
        }

        private string m_InputPrompt;
        public string InputPrompt
        {
            get => m_InputPrompt;
            set
            {
                m_InputPrompt = value ?? "Input:";
                InvokePropertyChanged(nameof(InputPrompt));
            }
        }

        private bool m_AllowMultiline;
        public TextWrapping TextBoxWrapping { get; private init; }
        public bool AllowMultiline
        {
            get => m_AllowMultiline;
            init
            {
                m_AllowMultiline = value;
                TextBoxWrapping = value ? TextWrapping.Wrap : TextWrapping.NoWrap;
            }
        }

        private Func<string, bool> m_InputValidator;
        #endregion

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;

        private void InvokePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Constructors
        public InputDialogView(string? inputPrompt = null, Func<string, bool> inputValidator = null)
        {
            m_InputText = "";
            m_InputPrompt = inputPrompt ?? "Input:";
            m_InputValidator = inputValidator ?? ((string x) => true);

            InitializeComponent();
        }
        #endregion

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_InputValidator(InputText))
                DialogResult = true;
            else
                MessageBox.Show("Invalid value!");
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InputBox.Focus();
            InputBox.Select(InputText.Length, 0);
        }
    }
}
