using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
        public event OperationResultEventHandler<INavigationTreeRoot>? DocumentDeleted;
        public event OperationResultEventHandler<INavigationTreeRoot>? CategoryDeleted;
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

            return Task.Run(() =>
            {
                lock (SyncRoot)
                {
                    try
                    {
                        if (!StorageModel.IsValidTitle(title))
                        {
                            InvokeCategoryCreated(OperationResultEventArgs.ErrorType.InvalidArgument, null, "Invalid title.");
                            return;
                        }

                        NavigationTreeCategory category = Organizer.CreateCategory(this, title);
                        AddCategory(category);
                        InvokeCategoryCreated(OperationResultEventArgs.ErrorType.None, category, null);
                    }
                    catch (Exception e)
                    {
                        InvokeCategoryCreated(OperationResultEventArgs.ErrorType.InternalError, null, e.Message);    
                    }
                }
            });
        }

        protected void InvokeCategoryCreated(OperationResultEventArgs.ErrorType errorType, INavigationTreeCategory? categoryInstance, string? errorText )
        {
            CategoryCreated?.Invoke(this, new CategoryCreatedEventArgs
            {
                CategoryInstance = categoryInstance,
                Error = errorType,
                ErrorText = errorText
            });
        }

        public Task CreateDocument(string title)
        {
            CheckDisposed();
            if (title == null)
                throw new ArgumentNullException(nameof(title));

            return Task.Run(() =>
            {
                lock (SyncRoot)
                {
                    try
                    {
                        if (!StorageModel.IsValidTitle(title))
                        {
                            InvokeDocumentCreated(OperationResultEventArgs.ErrorType.InvalidArgument, null, "Invalid title.");
                            return;
                        }

                        NavigationTreeDocument doc = Organizer.CreateDocument(this, title);
                        AddItem(doc);
                        InvokeDocumentCreated(OperationResultEventArgs.ErrorType.None, doc, null);
                    }
                    catch (Exception e)
                    {
                        InvokeDocumentCreated(OperationResultEventArgs.ErrorType.InternalError, null, e.Message);    
                    }
                }
            });
        }

        protected void InvokeDocumentCreated(OperationResultEventArgs.ErrorType errorType, INavigationTreeDocument? documentInstance, string? errorText )
        {
            DocumentCreated?.Invoke(this, new DocumentCreatedEventArgs
            {
                DocumentInstance = documentInstance,
                Error = errorType,
                ErrorText = errorText
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


            return Task.Run(() =>
            {
                lock (SyncRoot)
                {
                    try
                    {
                        Organizer.DeleteCategory(categoryInstance);
                        m_Categories.Remove(categoryInstance.Title);
                        m_Children.Remove(categoryInstance);
                        categoryInstance.Dispose();
                        InvokeCategoryDeleted(OperationResultEventArgs.ErrorType.None, null);
                    }
                    catch (Exception e)
                    {
                        InvokeCategoryDeleted(OperationResultEventArgs.ErrorType.InternalError, e.Message);
                    }
                }
            });
        }

        protected void InvokeCategoryDeleted(OperationResultEventArgs.ErrorType errorType, string? errorText )
        {
            CategoryDeleted?.Invoke(this, new OperationResultEventArgs
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

            return Task.Run(() =>
            {

            });
        }
        protected void InvokeDocumentDeleted(OperationResultEventArgs.ErrorType errorType, string? errorText )
        {
            DocumentDeleted?.Invoke(this, new OperationResultEventArgs
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
                if (!m_Categories.ContainsKey(category.Title))
                    m_Categories.Add(category.Title, category);
            
                m_Children.Add(category);
            }
        }

        public void AddItem(NavigationTreeNodeBase item)
        {
            CheckDisposed();
            lock (SyncRoot)
            {
                if (!m_Items.ContainsKey(item.Title))
                    m_Items.Add(item.Title, item);
                else
                    throw new ArgumentException($"Duplicate title: {item.Title}", nameof(item));

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
    }
}
