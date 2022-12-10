using System;
using System.Text;
using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.NavigationTree;

namespace DMOrganizerModel.Implementation.NavigationTree
{
    internal abstract class NavigationTreeNodeBase : OrganizerEntryBase, INavigationTreeNodeBase
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
        public NavigationTreeNodeBase(OrganizerModel organizer, INavigationTreeRootInternal parent, string title) : base(organizer)
        {
            m_Parent = parent;
            Title = title ?? throw new ArgumentOutOfRangeException(nameof(title));
        }
        #endregion

        #region Methods
        public abstract bool ChangeParent(INavigationTreeRoot newParent);
        public abstract bool Rename(string name);

        /// <summary>
        /// Is used to efficently construct path to this instance
        /// Specific implementation depends on the object (Section or Document)
        /// </summary>
        /// <param name="len">The number of characters required by previous steps of the algorithm to construct full path</param>
        /// <returns></returns>
        public virtual StringBuilder GetPath(int len = 0)
        {
            return Parent.GetPath(len + Title.Length + 1).Append('$').Append(Title);
        }

        public override void Dispose()
        {
            base.Dispose();
            m_Parent = null;
        }
        #endregion
    }
}
