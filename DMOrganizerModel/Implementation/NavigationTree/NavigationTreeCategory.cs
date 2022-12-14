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
        public string Title { get; private set; }

        private NavigationTreeRoot? m_Parent;
        public NavigationTreeRoot Parent
        {
            get
            {
                return m_Parent ?? throw new ObjectDisposedException(GetType().Name);
            }
            private set
            {
                m_Parent = value ?? throw new ArgumentNullException(nameof(Parent));
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

        #region Interface
        public Task Rename(string name)
        {
            CheckDisposed();
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return Task.Run(() =>
            {
                if (!StorageModel.IsValidTitle(name))
                {
                    InvokeRenamed(OperationResultEventArgs.ErrorType.InvalidArgument, $"The following title is not valid: {name}");
                    return;
                }

                lock (SyncRoot)
                {
                    string oldTitle = Title;
                    try
                    {
                        if (Parent.GetItem(name) != null)
                        {
                            InvokeRenamed(OperationResultEventArgs.ErrorType.InvalidArgument, $"A category with the same title is already present.");
                            return;
                        }
                        Title = name;
                        Organizer.ChangeCategoryTitle(this, name);
                        InvokeRenamed(OperationResultEventArgs.ErrorType.None, null);
                    }
                    catch (Exception e)
                    {
                        Title = oldTitle;
                        InvokeRenamed(OperationResultEventArgs.ErrorType.InternalError, e.Message);
                    }
                }
            });
        }

        public Task ChangeParent(INavigationTreeRoot newParent)
        {
            CheckDisposed();
            if (newParent == null)
                throw new ArgumentNullException(nameof(newParent));

            return Task.Run(() =>
            {
                lock (SyncRoot)
                {
                    NavigationTreeRoot oldParent = Parent;
                    try
                    {
                        if (newParent is NavigationTreeCategory category)
                        {
                            if (category.GetCategory(Title) != null)
                            {
                                InvokeParentChanged(OperationResultEventArgs.ErrorType.DuplicateTitle, $"Category {category.Title} already has a category named {Title}");
                                return;
                            }

                            Parent = category;
                            Organizer.ChangeCategoryParent(this, category.ItemID);
                        }
                        else if (newParent is NavigationTreeRoot root)
                        {
                            if (root.GetCategory(Title) != null)
                            {
                                InvokeParentChanged(OperationResultEventArgs.ErrorType.DuplicateTitle, $"The root already has a category named {Title}");
                                return;
                            }
                            Parent = root;
                            Organizer.ChangeCategoryParent(this, null);
                        }
                        else
                        {
                            InvokeParentChanged(OperationResultEventArgs.ErrorType.InvalidArgument, "Invalid parent type.");
                            return;
                        }
                        InvokeParentChanged(OperationResultEventArgs.ErrorType.None, null);
                    }
                    catch (Exception e)
                    {
                        Parent = oldParent;
                        InvokeParentChanged(OperationResultEventArgs.ErrorType.InternalError, e.Message);
                    }
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
