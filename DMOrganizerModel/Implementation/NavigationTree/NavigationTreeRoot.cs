using System;
using System.Collections.Generic;
using System.Text;
using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Content;
using DMOrganizerModel.Interface.NavigationTree;

namespace DMOrganizerModel.Implementation.NavigationTree
{
    internal class NavigationTreeRoot : OrganizerEntryBase, INavigationTreeRootInternal
    {
        #region Properties
        public IObservableReadOnlyCollection<INavigationTreeNodeBase> Children
        {
            get
            {
                return m_Children ?? throw new ObjectDisposedException(nameof(NavigationTreeRoot));
            }
        }
        #endregion

        #region Fields
        private Dictionary<string, NavigationTreeCategory>? m_Categories;
        private Dictionary<string, NavigationTreeNodeBase>? m_Items;
        private ObservableList<INavigationTreeNodeBase>? m_Children;
        #endregion

        #region Events
        public event OperationResultEventHandler<INavigationTreeRoot, DocumentCreatedEventArgs>? DocumentCreated;
        public event OperationResultEventHandler<INavigationTreeRoot, CategoryCreatedEventArgs>? CategoryCreated;
        public event OperationResultEventHandler<INavigationTreeRoot, DocumentDeletedEventArgs>? DocumentDeleted;
        public event OperationResultEventHandler<INavigationTreeRoot, DocumentDeletedEventArgs>? CategoryDeleted;
        #endregion

        #region Constructors
        public NavigationTreeRoot(OrganizerModel organizer) : base(organizer)
        {
            m_Children = new ObservableList<INavigationTreeNodeBase>();
            m_Categories = new Dictionary<string, NavigationTreeCategory>(new NoCaseStringComparer());
            m_Items = new Dictionary<string, NavigationTreeNodeBase>(new NoCaseStringComparer());
        }
        #endregion

        #region Methods
        public override void Dispose()
        {
            base.Dispose();
            m_Children = null;
            m_Categories = null;
            m_Items = null;
        }

        /// <summary>
        /// For initialization only, thread-unsafe
        /// </summary>
        
        public void AddCategory(NavigationTreeCategory category)
        {
            lock (SyncRoot)
            {
                if (m_Children == null || m_Categories == null)
                    throw new ObjectDisposedException(nameof(NavigationTreeRoot));

                if (!m_Categories.ContainsKey(category.Title))
                    m_Categories.Add(category.Title, category);
            
                m_Children.Add(category);
            }
        }

        public void AddItem(NavigationTreeNodeBase item)
        {
            lock (SyncRoot)
            {
                if (m_Children == null || m_Items == null)
                    throw new ObjectDisposedException(nameof(NavigationTreeRoot));

                if (!m_Items.ContainsKey(item.Title))
                    m_Items.Add(item.Title, item);
                else
                    throw new ArgumentException($"Duplicate title: {item.Title}", nameof(item));

                m_Children.Add(item);
            }    
        }

        public NavigationTreeCategory? GetCategory(string title)
        {
            if (m_Categories == null)
                throw new ObjectDisposedException(nameof(NavigationTreeRoot));

            m_Categories.TryGetValue(title, out NavigationTreeCategory? category);
            return category;
        }

        public NavigationTreeNodeBase? GetItem(string title)
        {
            if (m_Items == null)
                throw new ObjectDisposedException(nameof(NavigationTreeRoot));

            m_Items.TryGetValue(title, out NavigationTreeNodeBase? item);
            return item;
        }

        public bool CreateCategory(string title)
        {
            throw new NotImplementedException();
        }

        public bool CreateDocument(string title)
        {
            throw new NotImplementedException();
        }

        public bool DeleteCategory(INavigationTreeCategory category)
        {
            throw new NotImplementedException();
        }

        public bool DeleteDocument(IDocument document)
        {
            throw new NotImplementedException();
        }

        public StringBuilder GetPath(int len = 0)
        {
            return new StringBuilder(len);
        }
        #endregion
    }
}
