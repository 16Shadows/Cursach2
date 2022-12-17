using DMOrganizerApp.Resources;
using DMOrganizerApp.ViewModels;
using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface.NavigationTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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
using Windows.UI.Popups;

namespace DMOrganizerApp.Views
{
    /// <summary>
    /// Interaction logic for OrganizerView.xaml
    /// </summary>
    public partial class OrganizerView : UserControl
    {
        #region Internal types
        sealed class NavigationTreeTemplateSelector : DataTemplateSelector
        {
            private DataTemplate m_CategoryTemplate;
            private DataTemplate m_DocumentTemplate;
            private DataTemplate m_FallbackTemplate;

            public NavigationTreeTemplateSelector(DataTemplate categoryTemplate, DataTemplate documentTemplate, DataTemplate fallbackTemplate)
            {
                m_CategoryTemplate = categoryTemplate ?? throw new ArgumentNullException(nameof(categoryTemplate));
                m_DocumentTemplate = documentTemplate ?? throw new ArgumentNullException(nameof(documentTemplate));
                m_FallbackTemplate = fallbackTemplate ?? throw new ArgumentNullException(nameof(fallbackTemplate));
            }

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                if (item is INavigationTreeCategory)
                    return m_CategoryTemplate;
                else if (item is INavigationTreeDocument)
                    return m_DocumentTemplate;
                else
                    return m_FallbackTemplate;
            }
        }
        #endregion

        #region Constructors
        public OrganizerView()
        {
            InitializeComponent();
        }
        #endregion

        #region EventHandlers
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationTree.ItemTemplateSelector = new NavigationTreeTemplateSelector((DataTemplate)NavigationTree.Resources["CategoryTemplate"], (DataTemplate)NavigationTree.Resources["DocumentTemplate"], (DataTemplate)NavigationTree.Resources["FallbackTemplate"]);

            OrganizerViewModel viewModel = (OrganizerViewModel)DataContext;
            viewModel.LoadNavigationTree();
        }

        private void NavTreeCreateCategory_Click(object sender, RoutedEventArgs e)
        {
            InputDialogView inputDialog = new InputDialogView("New category name:", StorageModel.IsValidTitle)
            {
                Owner = Application.Current.MainWindow
            };
            if (inputDialog.ShowDialog() == true)
            {
                OrganizerViewModel viewModel = (OrganizerViewModel)DataContext;
                viewModel.CreateCategoryInRoot(inputDialog.InputText);
            }
        }

        private void NavTreeCreateDocument_Click(object sender, RoutedEventArgs e)
        {
            InputDialogView inputDialog = new InputDialogView("New category name:", StorageModel.IsValidTitle)
            {
                Owner = Application.Current.MainWindow
            };
            if (inputDialog.ShowDialog() == true)
            {
                OrganizerViewModel viewModel = (OrganizerViewModel)DataContext;
                viewModel.CreateDocumentInRoot(inputDialog.InputText);
            }
        }

        private void CategoryCreateCategory_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu menu = menuItem?.GetParent<ContextMenu>();
            INavigationTreeCategory category = (INavigationTreeCategory)menu.PlacementTarget?.GetVisualParent<TreeViewItem>()?.DataContext;
            InputDialogView inputDialog = new InputDialogView("New category name:", StorageModel.IsValidTitle)
            {
                Owner = Application.Current.MainWindow
            };
            if (inputDialog.ShowDialog() == true)
            {
                OrganizerViewModel viewModel = (OrganizerViewModel)DataContext;
                viewModel.CreateCategory(category, inputDialog.InputText);
            }
        }

        private void CategoryCreateDocument_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu menu = menuItem?.GetParent<ContextMenu>();
            INavigationTreeCategory category = (INavigationTreeCategory)menu.PlacementTarget?.GetVisualParent<TreeViewItem>()?.DataContext;
            InputDialogView inputDialog = new InputDialogView("New document name:", StorageModel.IsValidTitle)
            {
                Owner = Application.Current.MainWindow
            };
            if (inputDialog.ShowDialog() == true)
            {
                OrganizerViewModel viewModel = (OrganizerViewModel)DataContext;
                viewModel.CreateDocument(category, inputDialog.InputText);
            }
        }

        private void CategoryDelete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu menu = menuItem?.GetParent<ContextMenu>();
            INavigationTreeCategory category = (INavigationTreeCategory)menu.PlacementTarget?.GetVisualParent<TreeViewItem>()?.DataContext;
            OrganizerViewModel viewModel = (OrganizerViewModel)DataContext;
            viewModel.DeleteCategory(category);
        }

        private void CategoryRename_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu menu = menuItem?.GetParent<ContextMenu>();
            INavigationTreeCategory category = (INavigationTreeCategory)menu.PlacementTarget?.GetVisualParent<TreeViewItem>()?.DataContext;
            InputDialogView inputDialog = new InputDialogView("New category name:", StorageModel.IsValidTitle)
            {
                Owner = Application.Current.MainWindow,
                InputText = category.Title
            };
            if (inputDialog.ShowDialog() == true)
            {
                OrganizerViewModel viewModel = (OrganizerViewModel)DataContext;
                viewModel.RenameCategory(category, inputDialog.InputText);
            }
        }

        private void DocumentRename_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu menu = menuItem?.GetParent<ContextMenu>();
            INavigationTreeDocument document = (INavigationTreeDocument)menu.PlacementTarget?.GetVisualParent<TreeViewItem>()?.DataContext;
            InputDialogView inputDialog = new InputDialogView("New category name:", StorageModel.IsValidTitle)
            {
                Owner = Application.Current.MainWindow,
                InputText = document.Title
            };
            if (inputDialog.ShowDialog() == true)
            {
                OrganizerViewModel viewModel = (OrganizerViewModel)DataContext;
                viewModel.RenameDocument(document, inputDialog.InputText);
            }
        }

        private void DocumentDelete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu menu = menuItem?.GetParent<ContextMenu>();
            INavigationTreeDocument document = (INavigationTreeDocument)menu.PlacementTarget?.GetVisualParent<TreeViewItem>()?.DataContext;
            OrganizerViewModel viewModel = (OrganizerViewModel)DataContext;
            viewModel.DeleteDocument(document);
        }

        private void CategoryContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu panel = (ContextMenu)sender;
            TreeViewItem? treeItem = panel.PlacementTarget.GetVisualParent<TreeViewItem>();
            if (treeItem == null)
                throw new ArgumentException("Context menu was opened on a stack panel outside of navigation tree view", nameof(sender));
            treeItem.IsSelected = true;
        }

        private void DocumentContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu panel = (ContextMenu)sender;
            TreeViewItem? treeItem = panel.PlacementTarget.GetVisualParent<TreeViewItem>();
            if (treeItem == null)
                throw new ArgumentException("Context menu was opened on a stack panel outside of navigation tree view", nameof(sender));
            treeItem.IsSelected = true;
        }

        private void NavigationTreeContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu panel = (ContextMenu)sender;
            TreeView tree = (TreeView)panel.PlacementTarget;
            TreeViewItem? item = tree.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
            if (item != null)
            {
                item.IsSelected = true;
                item.IsSelected = false;
            }
        }

        private void DocumentOpen_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu menu = menuItem?.GetParent<ContextMenu>();
            INavigationTreeDocument document = (INavigationTreeDocument)menu.PlacementTarget?.GetVisualParent<TreeViewItem>()?.DataContext;
            OrganizerViewModel viewModel = (OrganizerViewModel)DataContext;
            viewModel.LoadDocument(document);
        }
        #endregion
    }
}
