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
        public OrganizerViewModel Organizer { get; }
        public ItemViewModel? SelectedItem { get; private set; }
        
        public ChooseReferenceItem(OrganizerViewModel organizer)
        {
            Organizer = organizer;
            InitializeComponent();
        }

        private void CanselButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            SelectedItem = (ItemViewModel)RefTree.SelectedValue;
            Close();
        }
    }

}
