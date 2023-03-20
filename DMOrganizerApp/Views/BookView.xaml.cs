using DMOrganizerViewModel;
using System.Windows;
using System.Windows.Controls;

namespace DMOrganizerApp.Views
{

    /// <summary>
    /// Логика взаимодействия для BookView.xaml
    /// </summary>
    public partial class BookView : UserControl
    {
        public BookView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is null || DataContext is not BookViewModel) { return; }
            else (DataContext as BookViewModel).ActivePageViewModel = (MVVMToolbox.ViewModel.ViewModelBase?)e.NewValue;
        }
    }
}
