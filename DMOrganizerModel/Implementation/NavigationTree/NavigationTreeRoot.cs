using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Content;
using DMOrganizerModel.Interface.NavigationTree;

namespace DMOrganizerModel.Implementation.NavigationTree
{
    internal class NavigationTreeRoot : OrganizerEntryBase, INavigationTreeRoot
    {
        #region Properties
        private ObservableList<INavigationTreeNodeBase>? m_Children;
        public IObservableReadOnlyCollection<INavigationTreeNodeBase> Children
        {
            get
            {
                CheckDisposed();
                return m_Children;
            }
        }
        #endregion

        #region Fields
        private Dictionary<string, NavigationTreeCategory>? m_Categories;
        private Dictionary<string, NavigationTreeNodeBase>? m_Items;
        #endregion

        #region Events
        public event OperationResultEventHandler<INavigationTreeRoot, DocumentCreatedEventArgs>? DocumentCreated;
        public event OperationResultEventHandler<INavigationTreeRoot, CategoryCreatedEventArgs>? CategoryCreated;
        public event OperationResultEventHandler<INavigationTreeRoot, DocumentDeletedEventArgs>? DocumentDeleted;
        public event OperationResultEventHandler<INavigationTreeRoot, CategoryDeletedEventArgs>? CategoryDeleted;
        #endregion

        #region Constructors
        public NavigationTreeRoot(OrganizerModel organizer) : base(organizer)
        {
            m_Children = new ObservableList<INavigationTreeNodeBase>();
            m_Categories = new Dictionary<string, NavigationTreeCategory>(new NoCaseStringComparer());
            m_Items = new Dictionary<string, NavigationTreeNodeBase>(new NoCaseStringComparer());
        }
        #endregion

        #region Interface
        public override void Dispose()
        {
            base.Dispose();
            
            foreach (var kvp in m_Categories)
                kvp.Value.Dispose();
            foreach (var kvp in m_Items)
                kvp.Value.Dispose();

            m_Items.Clear();
            m_Children.Clear();
            m_Items.Clear();

            m_Children = null;
            m_Categories = null;
            m_Items = null;
        }

        protected override void CheckDisposed()
        {
            base.CheckDisposed();
            if (m_Children == null || m_Categories == null || m_Items == null)
                throw new ObjectDisposedException(GetType().Name);
        }
        public Task CreateCategory(string title)
        {
            CheckDisposed();
            if (title == null)
                throw new ArgumentNullException(nameof(title));

            Dispatcher dispatch = Dispatcher.CurrentDispatcher;

            return Task.Run(() =>
            {
                try
                {
                    if (!StorageModel.IsValidTitle(title))
                    {
                        dispatch.BeginInvoke(() => InvokeCategoryCreated(OperationResultEventArgs.ErrorType.InvalidArgument, null, title, "Invalid title."));
                        return;
                    }
                        
                    NavigationTreeCategory category;
                    lock (SyncRoot)
                    {
                        if (m_Categories.ContainsKey(title))
                        {
                            dispatch.BeginInvoke(() => InvokeCategoryCreated(OperationResultEventArgs.ErrorType.DuplicateValue, null, title, "Duplicate title."));
                            return;
                        }

                        category = Organizer.CreateCategory(this, title);
                    }
                    dispatch.BeginInvoke(() =>
                    {
                        AddCategory(category);
                        InvokeCategoryCreated(OperationResultEventArgs.ErrorType.None, category, null, null);
                    });
                }
                catch (Exception e)
                {
                    dispatch.BeginInvoke(() => InvokeCategoryCreated(OperationResultEventArgs.ErrorType.InternalError, null, title, e.ToString()));
                }
            });
        }

        protected void InvokeCategoryCreated(OperationResultEventArgs.ErrorType errorType, INavigationTreeCategory? categoryInstance, string? title, string? errorText )
        {
            CategoryCreated?.Invoke(this, new CategoryCreatedEventArgs
            {
                CategoryInstance = categoryInstance,
                Error = errorType,
                ErrorText = errorText,
                Title = title
            });
        }

        public Task CreateDocument(string title)
        {
            CheckDisposed();
            if (title == null)
                throw new ArgumentNullException(nameof(title));

            Dispatcher dispatch = Dispatcher.CurrentDispatcher;

            return Task.Run(() =>
            {
                try
                {
                    if (!StorageModel.IsValidTitle(title))
                    {
                        dispatch.BeginInvoke(() => InvokeDocumentCreated(OperationResultEventArgs.ErrorType.InvalidArgument, null, title, "Invalid title."));
                        return;
                    }
                        
                    NavigationTreeDocument doc;
                    lock (SyncRoot)
                    {
                        if (m_Items.ContainsKey(title))
                        {
                            dispatch.BeginInvoke(() => InvokeDocumentCreated(OperationResultEventArgs.ErrorType.DuplicateValue, null, title, "Duplicate title."));
                            return;
                        }
                        doc = Organizer.CreateDocument(this, title);
                    }
                         
                    dispatch.BeginInvoke(() =>
                    {
                        AddItem(doc);
                        InvokeDocumentCreated(OperationResultEventArgs.ErrorType.None, doc, null, null);
                    });
                }
                catch (Exception e)
                {
                    dispatch.BeginInvoke(() => InvokeDocumentCreated(OperationResultEventArgs.ErrorType.InternalError, null, title, e.ToString()));
                }
            });
        }

        protected void InvokeDocumentCreated(OperationResultEventArgs.ErrorType errorType, INavigationTreeDocument? documentInstance, string? title, string? errorText )
        {
            DocumentCreated?.Invoke(this, new DocumentCreatedEventArgs
            {
                DocumentInstance = documentInstance,
                Error = errorType,
                ErrorText = errorText,
                Title = title
            });
        }

        public Task DeleteCategory(INavigationTreeCategory category)
        {
            CheckDisposed();
            if (category == null)
                throw new ArgumentNullException(nameof(category));
            else if (!m_Categories.ContainsKey(category.Title))
                throw new ArgumentException("Category doesn't belong to this node", nameof(category));

            if (category is not NavigationTreeCategory categoryInstance)
                throw new ArgumentException("Incompatiable instance", nameof(category));

            Dispatcher dispatch = Dispatcher.CurrentDispatcher;

            return Task.Run(() =>
            {
                string title = categoryInstance.Title;
                try
                {
                    lock (SyncRoot)
                        m_Categories.Remove(categoryInstance.Title);
                    Organizer.DeleteCategory(categoryInstance);
                    categoryInstance.PropertyChanged -= OnChildPropertyChanged;
                    dispatch.BeginInvoke(() =>
                    {
                        lock (SyncRoot)
                            m_Children.Remove(categoryInstance);
                        categoryInstance.Dispose();
                        InvokeCategoryDeleted(title, OperationResultEventArgs.ErrorType.None, null);
                    });
                }
                catch (Exception e)
                {
                    dispatch.BeginInvoke(() => InvokeCategoryDeleted(title, OperationResultEventArgs.ErrorType.InternalError, e.ToString()));
                }
            });
        }

        protected void InvokeCategoryDeleted(string title, OperationResultEventArgs.ErrorType errorType, string? errorText )
        {
            CategoryDeleted?.Invoke(this, new CategoryDeletedEventArgs(title)
            {
                Error = errorType,
                ErrorText = errorText
            });
        }

        public Task DeleteDocument(INavigationTreeDocument document)
        {
            CheckDisposed();
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            else if (!m_Items.ContainsKey(document.Title))
                throw new ArgumentException("Category doesn't belong to this node", nameof(document));

            if (document is not NavigationTreeDocument documentInstance)
                throw new ArgumentException("Incompatiable instance", nameof(document));

            Dispatcher dispatch = Dispatcher.CurrentDispatcher;

            return Task.Run(() =>
            {
                string title = documentInstance.Title;
                try
                {
                    lock (SyncRoot)
                        m_Categories.Remove(documentInstance.Title);
                    Organizer.DeleteDocument(documentInstance);
                    documentInstance.PropertyChanged += OnChildPropertyChanged;
                    dispatch.BeginInvoke(() =>
                    {
                        lock (SyncRoot)
                            m_Children.Remove(documentInstance);
                        documentInstance.Dispose();
                        InvokeCategoryDeleted(title, OperationResultEventArgs.ErrorType.None, null);
                    });
                }
                catch (Exception e)
                {
                    dispatch.BeginInvoke(() => InvokeCategoryDeleted(title, OperationResultEventArgs.ErrorType.InternalError, e.ToString()));
                }
            });
        }
        protected void InvokeDocumentDeleted(string title, OperationResultEventArgs.ErrorType errorType, string? errorText )
        {
            DocumentDeleted?.Invoke(this, new DocumentDeletedEventArgs(title)
            {
                Error = errorType,
                ErrorText = errorText
            });
        }
        #endregion

        #region Methods

        public void AddCategory(NavigationTreeCategory category)
        {
            CheckDisposed();
            lock (SyncRoot)
            {
                if (m_Categories.ContainsKey(category.Title))
                    throw new ArgumentException("Duplicate category title", nameof(category));

                category.PropertyChanged += OnChildPropertyChanged;
                m_Categories.Add(category.Title, category);
                m_Children.Add(category);
            }
        }

        public void AddItem(NavigationTreeNodeBase item)
        {
            CheckDisposed();
            lock (SyncRoot)
            {
                if (m_Items.ContainsKey(item.Title))
                    throw new ArgumentException("Duplicate category title", nameof(item));

                item.PropertyChanged += OnChildPropertyChanged;
                m_Items.Add(item.Title, item);
                m_Children.Add(item);
            }    
        }

        public NavigationTreeCategory? GetCategory(string title)
        {
            CheckDisposed();

            m_Categories.TryGetValue(title, out NavigationTreeCategory? category);
            return category;
        }

        public NavigationTreeNodeBase? GetItem(string title)
        {
            CheckDisposed();

            m_Items.TryGetValue(title, out NavigationTreeNodeBase? item);
            return item;
        }

        public virtual StringBuilder GetPath(int len = 0)
        {
            return new StringBuilder(len);
        }
        #endregion

        #region EventHandlers
        protected void OnChildPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Title")
                return;

            if (sender is NavigationTreeCategory category)
            {
                string key = m_Categories.FirstOrDefault(kvp => kvp.Value == category).Key;
                if (key != null)
                    m_Categories.Remove(key);
                m_Categories.Add(category.Title, category);
            }
            else if (sender is NavigationTreeNodeBase item)
            {
                string key = m_Items.FirstOrDefault(kvp => kvp.Value == item).Key;
                if (key != null)
                    m_Items.Remove(key);
                m_Items.Add(item.Title, item);
            }
        }
        #endregion
    }
}
