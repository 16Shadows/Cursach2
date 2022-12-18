using DMOrganizerApp.Resources;
using DMOrganizerApp.ViewModels;
using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface.Content;
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
    /// Interaction logic for DocumentView.xaml
    /// </summary>
    public partial class DocumentView : UserControl
    {
        public DocumentView()
        {
            InitializeComponent();
        }

        private void AddTag_Click(object sender, RoutedEventArgs e)
        {
            InputDialogView inputDialog = new InputDialogView("Tag name: ", x => x.Length > 0);
            if (inputDialog.ShowDialog() == true)
            {
                DocumentViewModel viewModel = (DocumentViewModel)DataContext;
                viewModel.AddTag(inputDialog.InputText);
            }    
        }

        private void RemoveTag_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            string tag = (string)(menuItem.GetParent<ContextMenu>()?.PlacementTarget as Border).DataContext;
            DocumentViewModel viewModel = (DocumentViewModel)DataContext;
            viewModel.RemoveTag(tag);
        }

        private void EditTitle_Click(object sender, RoutedEventArgs e)
        {
            Button senderButton = (Button)sender;
            ISection section = (ISection)senderButton.DataContext;
            InputDialogView inputDialog = new InputDialogView("Title: ", StorageModel.IsValidTitle)
            {
                InputText = section.Title
            };
            if (inputDialog.ShowDialog() == true)
            {
                DocumentViewModel viewModel = (DocumentViewModel)DataContext;
                viewModel.EditTitle(section, inputDialog.InputText);
            }
        }

        private void EditContent_Click(object sender, RoutedEventArgs e)
        {
            Button senderButton = (Button)sender;
            ISection section = (ISection)senderButton.DataContext;
            InputDialogView inputDialog = new InputDialogView("Content: ")
            {
                AllowMultiline = true,
                InputText = section.Content
            };
            if (inputDialog.ShowDialog() == true)
            {
                DocumentViewModel viewModel = (DocumentViewModel)DataContext;
                viewModel.EditContent(section, inputDialog.InputText);
            }
        }

        private void RemoveSection_Click(object sender, RoutedEventArgs e)
        {
            Button senderButton = (Button)sender;
            ISection section = (ISection)senderButton.DataContext;
            DocumentViewModel viewModel = (DocumentViewModel)DataContext;
            viewModel.RemoveSection(section);
        }

        private void AddSection_Click(object sender, RoutedEventArgs e)
        {
            Button senderButton = (Button)sender;
            ISection section = (ISection)senderButton.DataContext;
            InputDialogView inputDialog = new InputDialogView("Title: ", StorageModel.IsValidTitle);
            if (inputDialog.ShowDialog() == true)
            {
                DocumentViewModel viewModel = (DocumentViewModel)DataContext;
                viewModel.AddSection(section, inputDialog.InputText);
            }
        }
    }
}
