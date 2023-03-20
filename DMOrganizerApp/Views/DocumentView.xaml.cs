using DMOrganizerApp.UserControls;
using DMOrganizerViewModel;
using MVVMToolbox.Command;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DMOrganizerApp.Views
{
    /// <summary>
    /// Interaction logic for DocumentView.xaml
    /// </summary>
    public partial class DocumentView : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void InvokePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool m_EditMode;
        public bool EditMode
        {
            get => m_EditMode;
            set
            {
                if (m_EditMode == value)
                    return;
                m_EditMode = value;
                InvokePropertyChanged(nameof(EditMode));
            }
        }

        public DeferredCommand BeginEdit { get; }
        public DeferredCommand EndEdit { get; }

        public DocumentView()
        {
            BeginEdit = new DeferredCommand(CommandHandler_BeginEdit , CanExecute_BeginEdit);
            EndEdit = new DeferredCommand(CommandHandler_EndEdit, CanExecute_BeginEdit);

            InitializeComponent();
        }

        private void CommandHandler_BeginEdit()
        {
            EditMode = true;
        }

        private void CommandHandler_EndEdit()
        {
            EditMode = false;
            ContentEditBox.GetBindingExpression(FormattableRichTextBox.DocumentProperty).UpdateSource();
        }

        private bool CanExecute_BeginEdit()
        {
            return !((SectionViewModel)DataContext).LockingOperation;
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is SectionViewModel old)
                old.PropertyChanged -= DataContext_PropertyChanged;
            if (e.NewValue is SectionViewModel model)
                model.PropertyChanged += DataContext_PropertyChanged;
        }

        private void DataContext_PropertyChanged(object? source, PropertyChangedEventArgs e)
        {
            SectionViewModel vm = (SectionViewModel)source;
            if (e.PropertyName != nameof(vm.LockingOperation))
                return;
            BeginEdit.InvokeCanExecuteChanged();
            EndEdit.InvokeCanExecuteChanged();
        }

        private void ContentEditBox_LostFocus(object sender, RoutedEventArgs e)
        {
            EditMode = false;
        }
    }
}
