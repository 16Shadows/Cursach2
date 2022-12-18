using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.NavigationTree;

namespace DMOrganizerModel.Implementation.NavigationTree
{
    internal abstract class NavigationTreeNodeBase : OrganizerEntryBase, INavigationTreeNodeBase
    {
        #region Properties
        private string m_Title;
        public string Title
        {
            get => m_Title;
            protected set
            {
                CheckDisposed();
                if (m_Title == value)
                    return;
                m_Title = value ?? throw new ArgumentNullException(nameof(Title));
                InvokePropertyChanged(nameof(Title));
            }
        }
        private NavigationTreeRoot? m_Parent;
        public NavigationTreeRoot Parent
        {
            get
            {
                return m_Parent ?? throw new ObjectDisposedException(GetType().Name);
            }
            protected set
            {
                CheckDisposed();
                if (m_Parent == value)
                    return;
                m_Parent = value ?? throw new ArgumentNullException(nameof(Parent));
                InvokePropertyChanged(nameof(Parent));
            }
        }
        INavigationTreeRoot INavigationTreeNodeBase.Parent => Parent;
        public int ItemID { get; }
        #endregion

        #region Events
        public event OperationResultEventHandler<INavigationTreeNodeBase>? Renamed;
        public event OperationResultEventHandler<INavigationTreeNodeBase>? ParentChanged;
        
        protected void InvokeRenamed(OperationResultEventArgs.ErrorType errorType, string? errorText)
        {
            Renamed?.Invoke(this, new OperationResultEventArgs
            {
                Error = errorType,
                ErrorText = errorText
            });
        }

        protected void InvokeParentChanged(OperationResultEventArgs.ErrorType errorType, string? errorText)
        {
            ParentChanged?.Invoke(this, new OperationResultEventArgs
            {
                Error = errorType,
                ErrorText = errorText
            });
        }
        #endregion

        #region Constructors
        public NavigationTreeNodeBase(OrganizerModel organizer, NavigationTreeRoot parent, string title, int itemid) : base(organizer)
        {
            ItemID = itemid;
            m_Parent = parent;
            m_Title = title ?? throw new ArgumentOutOfRangeException(nameof(title));
        }
        #endregion

        #region Interface
        public abstract Task ChangeParent(INavigationTreeRoot newParent);
        public abstract Task Rename(string name);

        public override void Dispose()
        {
            base.Dispose();
            m_Parent = null;
        }
        protected override void CheckDisposed()
        {
            base.CheckDisposed();
            if (m_Parent == null)
                throw new ObjectDisposedException(GetType().Name);
        }
        #endregion

        #region Methods
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
        #endregion
    }
}
