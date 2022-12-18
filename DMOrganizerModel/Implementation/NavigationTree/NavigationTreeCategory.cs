using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.NavigationTree;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DMOrganizerModel.Implementation.NavigationTree
{
    internal sealed class NavigationTreeCategory : NavigationTreeRoot, INavigationTreeCategory
    {
        #region Properties
        private string m_Title;
        public string Title
        {
            get => m_Title;
            private set
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
            private set
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
        #endregion

        #region Constructors
        public NavigationTreeCategory(OrganizerModel organizer, NavigationTreeRoot parent, string title, int itemID) : base(organizer)
        {
            m_Title = title;
            m_Parent = parent;
            ItemID = itemID;
        }
        #endregion

        #region Interface
        public Task Rename(string name)
        {
            CheckDisposed();
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

            return Task.Run(() =>
            {
                if (!StorageModel.IsValidTitle(name))
                {
                    dispatcher.BeginInvoke(() => InvokeRenamed(OperationResultEventArgs.ErrorType.InvalidArgument, $"The following title is not valid: {name}"));
                    return;
                }   
                
                string oldTitle = null;
                try
                {        
                    lock (SyncRoot)
                    {
                        if (Parent.GetItem(name) != null)
                        {
                            dispatcher.BeginInvoke(() => InvokeRenamed(OperationResultEventArgs.ErrorType.DuplicateValue, "A category with the same title is already present."));
                            return;
                        }
                        oldTitle = Title;
                    }
                    Organizer.ChangeCategoryTitle(this, name);
                    dispatcher.BeginInvoke(() =>
                    {
                        lock (SyncRoot)
                            Title = name;
                        InvokeRenamed(OperationResultEventArgs.ErrorType.None, null);
                    });
                }
                catch (Exception e)
                {
                    dispatcher.BeginInvoke(() =>
                    {
                        lock (SyncRoot)
                            if (oldTitle != null && oldTitle != Title)
                                Title = oldTitle;
                        InvokeRenamed(OperationResultEventArgs.ErrorType.InternalError, e.ToString());
                    });
                }
            });
        }

        public Task ChangeParent(INavigationTreeRoot newParent)
        {
            CheckDisposed();
            if (newParent == null)
                throw new ArgumentNullException(nameof(newParent));
            
            if (newParent is not NavigationTreeRoot root)
                throw new ArgumentException("Incompatiable instance", nameof(newParent));

            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

            return Task.Run(() =>
            {
                NavigationTreeRoot oldParent = null;
                try
                {
                    lock (SyncRoot)
                    {
                        oldParent = Parent;
                        if (root.GetItem(Title) != null)
                        {
                            dispatcher.BeginInvoke(() => InvokeParentChanged(OperationResultEventArgs.ErrorType.DuplicateValue, $"The parent already has an item named {Title}"));
                            return;
                        }
                    }
                    Organizer.ChangeCategoryParent(this, root as NavigationTreeCategory);
                    dispatcher.BeginInvoke(() =>
                    {
                        lock (SyncRoot)
                            Parent = root;
                        InvokeParentChanged(OperationResultEventArgs.ErrorType.None, null);
                    });
                }
                catch (Exception e)
                {
                    dispatcher.BeginInvoke(() =>
                    {
                        lock (SyncRoot)
                            if (oldParent != null && oldParent != Parent)
                                Parent = oldParent;
                        InvokeParentChanged(OperationResultEventArgs.ErrorType.InternalError, e.ToString());
                    });
                }
            });
        }

        private void InvokeRenamed(OperationResultEventArgs.ErrorType errorType, string? errorText)
        {
            Renamed?.Invoke(this, new OperationResultEventArgs
            {
                Error = errorType,
                ErrorText = errorText
            });
        }

        private void InvokeParentChanged(OperationResultEventArgs.ErrorType errorType, string? errorText)
        {
            ParentChanged?.Invoke(this, new OperationResultEventArgs
            {
                Error = errorType,
                ErrorText = errorText
            });
        }
        #endregion

        #region Methods
        public override StringBuilder GetPath(int len = 0)
        {
            return Parent.GetPath(len + Title.Length + 1).Append('/').Append(Title);
        }
        #endregion
    }
}
