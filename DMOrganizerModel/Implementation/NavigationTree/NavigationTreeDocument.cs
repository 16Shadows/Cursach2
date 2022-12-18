using DMOrganizerModel.Implementation.Content;
using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.NavigationTree;
using System.Threading.Tasks;
using System;
using DMOrganizerModel.Interface.Content;
using System.Windows.Threading;
using System.Text.Json.Serialization;
using System.Windows.Media.Animation;

namespace DMOrganizerModel.Implementation.NavigationTree
{
    internal sealed class NavigationTreeDocument : NavigationTreeNodeBase, INavigationTreeDocument
    {
        #region Properties
        private Document? m_DocumentInstance;
        #endregion

        #region Events
        public event OperationResultEventHandler<INavigationTreeDocument, DocumentLoadedEventArgs>? DocumentLoaded;
        #endregion

        #region Constructors
        public NavigationTreeDocument(OrganizerModel organizer, NavigationTreeRoot parent, string title, int itemid) : base(organizer, parent, title, itemid) {}
        #endregion

        #region Interface
        public override Task ChangeParent(INavigationTreeRoot newParent)
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
                    Organizer.ChangeDocumentParent(this, root as NavigationTreeCategory);
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

        public Task LoadDocument()
        {
            CheckDisposed();

            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

            return Task.Run(() =>
            {
                Document? doc = null;
                try
                {
                    lock (SyncRoot)
                    {
                        if (m_DocumentInstance == null)
                            doc = m_DocumentInstance = GetDocument();
                        else
                            doc = m_DocumentInstance;
                    }
                    dispatcher.BeginInvoke(() => InvokeDocumentLoaded(OperationResultEventArgs.ErrorType.None, doc, null));
                }
                catch (Exception e)
                {
                    doc?.Dispose();
                    dispatcher.BeginInvoke(() => InvokeDocumentLoaded(OperationResultEventArgs.ErrorType.InvalidArgument, doc, e.ToString()));
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
                            dispatcher.BeginInvoke(() => InvokeRenamed(OperationResultEventArgs.ErrorType.DuplicateValue, "An item with the same title is already present."));
                            return;
                        }
                        oldTitle = Title;
                    }
                    Organizer.ChangeDocumentTitle(this, name);
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
        #endregion

        #region Methods
        public Document GetDocument()
        {
            Document doc = Organizer.LoadDocument(this);
            doc.PropertyChanged += Doc_PropertyChanged;
            return doc;
        }

        #endregion

        #region EventHandlers
        private void Doc_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            lock (SyncRoot)
            {
                if (sender == m_DocumentInstance && e.PropertyName == nameof(Title))
                    Title = m_DocumentInstance.Title;
            }
        }
        #endregion
    }
}
