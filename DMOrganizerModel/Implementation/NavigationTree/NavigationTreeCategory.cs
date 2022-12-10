using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Content;
using DMOrganizerModel.Interface.NavigationTree;
using System;

namespace DMOrganizerModel.Implementation.NavigationTree
{
    internal sealed class NavigationTreeCategory : NavigationTreeRoot, INavigationTreeCategory
    {
        #region Properties
        public string Title { get; }

        private INavigationTreeRootInternal? m_Parent;
        public INavigationTreeRootInternal Parent
        {
            get
            {
                return m_Parent ?? throw new ObjectDisposedException(GetType().Name);
            }
        }
        INavigationTreeRoot INavigationTreeNodeBase.Parent => Parent;
        #endregion

        #region Events
        public event OperationResultEventHandler<INavigationTreeNodeBase>? Renamed;
        public event OperationResultEventHandler<INavigationTreeNodeBase>? ParentChanged;
        #endregion

        #region Constructors
        public NavigationTreeCategory(OrganizerModel organizer, INavigationTreeRootInternal parent, string title) : base(organizer)
        {
            Title = title;
            m_Parent = parent;
        }
        #endregion

        #region Methods
        public bool CreateCategory(string title)
        {
            throw new System.NotImplementedException();
        }

        public bool CreateDocument(string title)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteCategory(INavigationTreeCategory category)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteDocument(IDocument document)
        {
            throw new System.NotImplementedException();
        }

        public bool Rename(string name)
        {
            throw new NotImplementedException();
        }

        public bool ChangeParent(INavigationTreeRoot newParent)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
