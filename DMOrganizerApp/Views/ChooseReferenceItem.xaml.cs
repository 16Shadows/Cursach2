using DMOrganizerViewModel;
using MVVMToolbox.ViewModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DMOrganizerApp.Views
{
    /// <summary>
    /// Логика взаимодействия для ChooseReferenceItem.xaml
    /// </summary>
    public partial class ChooseReferenceItem : Window
    {
        public OrganizerViewModel Organizer;
        public ItemViewModel SelectedItem = null;
        public ChooseReferenceItem()
        {
            InitializeComponent();
        }
        public ChooseReferenceItem(OrganizerViewModel organizer)
        {
            Organizer = organizer;
            InitializeComponent();
        }
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is null || DataContext is not ContainerObjectViewModel || e.NewValue is not ItemViewModel) { return; }
            else SelectedItem = (ItemViewModel)e.NewValue;

            if (e.NewValue is DocumentViewModel || e.NewValue is SectionViewModel) this.SetButton.IsEnabled = true;
            else this.SetButton.IsEnabled = false;

            //if (DataContext is null || DataContext is not ContainerObjectViewModel || e.NewValue is not MVVMToolbox.ViewModel.ViewModelBase) { return; }
            //else (DataContext as ContainerObjectViewModel).ActivePageViewModel = (MVVMToolbox.ViewModel.ViewModelBase?)e.NewValue;

            //if (e.NewValue is DocumentViewModel || e.NewValue is SectionViewModel) this.SetButton.IsEnabled = true;
            //else this.SetButton.IsEnabled = false;
        }
    }

}
