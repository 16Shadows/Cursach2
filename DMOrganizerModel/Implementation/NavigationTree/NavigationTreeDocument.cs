using DMOrganizerModel.Implementation.Content;
using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.NavigationTree;
using System.Threading.Tasks;
using System;
using DMOrganizerModel.Interface.Content;

namespace DMOrganizerModel.Implementation.NavigationTree
{
    internal sealed class NavigationTreeDocument : NavigationTreeNodeBase, INavigationTreeDocument
    {
        #region Events
        public event OperationResultEventHandler<INavigationTreeDocument, DocumentLoadedEventArgs>? DocumentLoaded;
        #endregion

        #region Constructors
        public NavigationTreeDocument(OrganizerModel organizer, NavigationTreeRoot parent, string title, int itemid) : base(organizer, parent, title, itemid) {}
        #endregion

        public Document GetDocument()
        {
            return Organizer.LoadDocument(this);
        }

        public override Task ChangeParent(INavigationTreeRoot newParent)
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
                            if (category.GetItem(Title) != null)
                            {
                                InvokeParentChanged(OperationResultEventArgs.ErrorType.DuplicateTitle, $"Category {category.Title} already has an item named {Title}");
                                return;
                            }

                            Parent = category;
                            Organizer.ChangeDocumentParent(this, category.ItemID);
                        }
                        else if (newParent is NavigationTreeRoot root)
                        {
                            if (root.GetItem(Title) != null)
                            {
                                InvokeParentChanged(OperationResultEventArgs.ErrorType.DuplicateTitle, $"The root already has an item named {Title}");
                                return;
                            }
                            Parent = root;
                            Organizer.ChangeDocumentParent(this, null);
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

        public Task LoadDocument()
        {
            CheckDisposed();
            return Task.Run(() =>
            {
                Document? doc = null;
                try
                {
                    doc = GetDocument();
                    InvokeDocumentLoaded(OperationResultEventArgs.ErrorType.None, doc, null);
                }
                catch (Exception e)
                {
                    InvokeDocumentLoaded(OperationResultEventArgs.ErrorType.InvalidArgument, doc, e.Message);
                }
            });
        }

        private void InvokeDocumentLoaded(OperationResultEventArgs.ErrorType errorType, IDocument? document, string? errorString)
        {
            DocumentLoaded?.Invoke(this, new DocumentLoadedEventArgs
            {
                DocumentInstance = document,
                Error = errorType,
                ErrorText = errorString
            });
        }

        public override Task Rename(string name)
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
                            InvokeRenamed(OperationResultEventArgs.ErrorType.InvalidArgument, $"An item with the same title is already present.");
                            return;
                        }
                        Title = name;
                        Organizer.ChangeDocumentTitle(this, name);
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
    }
}
