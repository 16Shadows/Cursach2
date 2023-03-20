using ClockApp.Utility.Context;
using DMOrganizerApp.Services;
using DMOrganizerApp.ViewModels;
using System.Windows;

namespace DMOrganizerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainViewModel(new WPFContext(Dispatcher), new DMOrganizerAppServiceProvider()); 
            InitializeComponent();
        }
    }
}
