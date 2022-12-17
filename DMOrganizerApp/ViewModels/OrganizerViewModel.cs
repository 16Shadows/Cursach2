using System;
using DMOrganizerModel.Interface.NavigationTree;
using DMOrganizerModel.Interface.Model;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Linq;

namespace DMOrganizerApp.ViewModels
{
    internal class OrganizerViewModel : ViewModelHost
    {
        #region Properties
        private INavigationTreeRoot? m_NavigationTreeRoot;
        public INavigationTreeRoot NavigationTreeRoot
        {
            get => m_NavigationTreeRoot;
            set
            {
                m_NavigationTreeRoot = value ?? throw new ArgumentNullException(nameof(NavigationTreeRoot));
                InvokePropertyChanged(nameof(NavigationTreeRoot));
            }
        }
        #endregion

        #region Fields
        private IOrganizerModel m_OrganizerModel;
        #endregion

        #region Constructor
        public OrganizerViewModel(IOrganizerModel organizer) : base(null)
        {
            m_OrganizerModel = organizer ?? throw new ArgumentNullException(nameof(organizer));
            m_OrganizerModel.NavigationTreeReceived += OrganizerModel_NavigationTreeReceived;
        }

        ~OrganizerViewModel()
        {
            if (NavigationTreeRoot != null)
                RecursiveUnsubscribe(NavigationTreeRoot);
        }
        #endregion

        #region Methods
        public void LoadNavigationTree()
        {
            if (NavigationTreeRoot != null)
                RecursiveUnsubscribe(NavigationTreeRoot);

            m_OrganizerModel.GetNavigationTree();
        }

        public void CreateCategoryInRoot(string name)
        {
            if (m_NavigationTreeRoot == null)
                throw new InvalidOperationException("Tree root has not been loaded yet.");

            m_NavigationTreeRoot.CreateCategory(name);
        }

        public void CreateCategory(INavigationTreeCategory parent, string name)
        {
            parent.CreateCategory(name);
        }

        public void RenameCategory(INavigationTreeCategory category, string name)
        {
            category.Rename(name);
        }

        public void DeleteCategory(INavigationTreeCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            RecursiveUnsubscribe(category);
            category.Parent.DeleteCategory(category);
        }

        public void CreateDocumentInRoot(string name)
        {
            if (m_NavigationTreeRoot == null)
                throw new InvalidOperationException("Tree root has not been loaded yet.");

            m_NavigationTreeRoot.CreateDocument(name);
        }

        public void CreateDocument(INavigationTreeCategory parent, string name)
        {
            parent.CreateDocument(name);
        }

        public void RenameDocument(INavigationTreeDocument document, string name)
        {
            document.Rename(name);
        }

        public void DeleteDocument(INavigationTreeDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            document.ParentChanged += NavTreeItem_ParentChanged;
            document.Renamed += NavTreeItem_Renamed;
            document.Parent.DeleteDocument(document);
        }

        public void LoadDocument(INavigationTreeDocument document)
        {
            document.DocumentLoaded += DocumentLoaded;
            document.LoadDocument();
        }

        private void RecursiveSubscribe(INavigationTreeRoot root)
        {
            root.CategoryCreated += CategoryCreated;
            root.CategoryDeleted += CategoryDeleted;
            root.DocumentCreated += DocumentCreated;
            root.DocumentDeleted += DocumentDeleted;

            foreach (INavigationTreeNodeBase node in root.Children)
            {
                if (node is INavigationTreeCategory category)
                    RecursiveSubscribe(category);
                
                node.ParentChanged += NavTreeItem_ParentChanged;
                node.Renamed += NavTreeItem_Renamed;
            }
        }

        private void RecursiveUnsubscribe(INavigationTreeRoot root)
        {
            root.CategoryCreated -= CategoryCreated;
            root.CategoryDeleted -= CategoryDeleted;
            root.DocumentCreated -= DocumentCreated;
            root.DocumentDeleted -= DocumentDeleted;

            foreach (INavigationTreeNodeBase node in root.Children)
            {
                if (node is INavigationTreeCategory category)
                    RecursiveSubscribe(category);
                
                node.ParentChanged -= NavTreeItem_ParentChanged;
                node.Renamed -= NavTreeItem_Renamed;
            }
        }

        #endregion

        #region EventHandlers
        private void DocumentLoaded(INavigationTreeDocument sender, DocumentLoadedEventArgs e)
        {
            sender.DocumentLoaded -= DocumentLoaded;
            if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.None)
            {
                if (ActiveViewModel is DocumentViewModel documentViewModel)
                    documentViewModel.Document = e.DocumentInstance;
                else
                    ActiveViewModel = new DocumentViewModel(e.DocumentInstance);
            }
            else
                MessageBox.Show(e.ErrorText);
        }

        private void OrganizerModel_NavigationTreeReceived(IOrganizerModel sender, NavigationTreeReceivedEventArgs e)
        {
            if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.None)
            {
                NavigationTreeRoot = e.NavigationTree;
                RecursiveSubscribe(NavigationTreeRoot);
            }
            else
            {
                MessageBox.Show( e.Error.ToString() + " " + e.ErrorText);
            }
        }

        private void NavTreeItem_Renamed(INavigationTreeNodeBase sender, DMOrganizerModel.Interface.OperationResultEventArgs e)
        {
            if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.None)
                MessageBox.Show("Renamed!");
            else if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.DuplicateTitle)
                MessageBox.Show("Duplicate title!");
            else
                MessageBox.Show(e.ErrorText);
        }

        private void NavTreeItem_ParentChanged(INavigationTreeNodeBase sender, DMOrganizerModel.Interface.OperationResultEventArgs e)
        {
            if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.None)
                MessageBox.Show("Moved!");
            else
                MessageBox.Show(e.ErrorText);
        }

        private void DocumentDeleted(INavigationTreeRoot sender, DocumentDeletedEventArgs e)
        {
            if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.None)
                MessageBox.Show("Deleted!");
            else
            {
                INavigationTreeDocument doc = (INavigationTreeDocument)sender.Children.First(x => (x is INavigationTreeDocument cat && cat.Title.Equals(e.Title)));
                doc.ParentChanged += NavTreeItem_ParentChanged;
                doc.Renamed += NavTreeItem_Renamed;
                MessageBox.Show(e.ErrorText);
            }
        }

        private void DocumentCreated(INavigationTreeRoot sender, DocumentCreatedEventArgs e)
        {
            if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.None)
            {
                MessageBox.Show("Created!");
                e.DocumentInstance.ParentChanged += NavTreeItem_ParentChanged;
                e.DocumentInstance.Renamed += NavTreeItem_Renamed;
            }
            else if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.DuplicateTitle)
                MessageBox.Show("Duplicate title: " + e.Title);
            else
                MessageBox.Show(e.ErrorText);
        }

        private void CategoryDeleted(INavigationTreeRoot sender, CategoryDeletedEventArgs e)
        {
            if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.None)
                MessageBox.Show("Deleted!");
            else
            {
                RecursiveSubscribe((INavigationTreeCategory)sender.Children.First(x => (x is INavigationTreeCategory cat && cat.Title.Equals(e.Title))));
                MessageBox.Show(e.ErrorText);
            }
        }

        private void CategoryCreated(INavigationTreeRoot sender, CategoryCreatedEventArgs e)
        {
            if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.None)
            {
                MessageBox.Show("Created!");
                RecursiveSubscribe(e.CategoryInstance);
            }
            else if (e.Error == DMOrganizerModel.Interface.OperationResultEventArgs.ErrorType.DuplicateTitle)
                MessageBox.Show("Duplicate title: " + e.Title);
            else
                MessageBox.Show(e.ErrorText);
        }
        #endregion
    }
}
