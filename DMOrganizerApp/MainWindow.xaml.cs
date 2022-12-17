using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Model;
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

using DMOrganizerApp.ViewModels;
using DMOrganizerModel.Implementation.Model;

namespace DMOrganizerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected IOrganizerModel m_Organizer;

        public MainWindow()
        {
            InitializeComponent();

            m_Organizer = StorageModel.LoadOrganizer("test.dmo");

            DataContext = new ViewModelHost(new OrganizerViewModel(m_Organizer));
        }

        ~MainWindow()
        {
            StorageModel.UnloadOrganizer(m_Organizer);
        }
    }
}
