using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.NavigationTree;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.NavigationTree
{
    internal sealed class NavigationTreeCategory : NavigationTreeRoot, INavigationTreeCategory
    {
        #region Properties
        public string Title { get; }

        private NavigationTreeRoot? m_Parent;
        public NavigationTreeRoot Parent
        {
            get
            {
                return m_Parent ?? throw new ObjectDisposedException(GetType().Name);
            }
        }
        INavigationTreeRoot INavigationTreeNodeBase.Parent => Parent;
        public int ItemID { get; }
        #endregion

        #region Events
        public event OperationResultEventHandler<INavigationTreeNodeBase>? Renamed;
        public event OperationResultEventHandler<INavigationTreeNodeBase>? ParentChanged;
        #endregion

        #region Constructors
        public NavigationTreeCategory(OrganizerModel organizer, NavigationTreeRoot parent, string title, int itemID) : base(organizer)
        {
            Title = title;
            m_Parent = parent;
            ItemID = itemID;
        }
        #endregion

        #region Methods
        public Task Rename(string name)
        {
            throw new NotImplementedException();
        }

        public Task ChangeParent(INavigationTreeRoot newParent)
        {
            throw new NotImplementedException();
        }

        public override StringBuilder GetPath(int len = 0)
        {
            return Parent.GetPath(len + Title.Length + 1).Append('/').Append(Title);
        }
        #endregion
    }
}
