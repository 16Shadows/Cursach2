using System;
using System.Text;

using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Content;
using DMOrganizerModel.Implementation.NavigationTree;
using DMOrganizerModel.Implementation.Model;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Threading;
using System.Linq;

namespace DMOrganizerModel.Implementation.Content
{
    internal sealed class Document : SectionBase, IDocument
    {
        #region Properties
        private ObservableList<string>? m_Tags;
        public ObservableList<string> Tags
        {
            get
            {
                CheckDisposed();
                return m_Tags;
            }
            private set
            {
                CheckDisposed();
                m_Tags = value ?? throw new ArgumentNullException(nameof(Tags));
            }
        }
        IObservableReadOnlyCollection<string> IDocument.Tags => Tags;

        private NavigationTreeDocument? m_NavigationEntry;
        public NavigationTreeDocument NavigationEntry
        {
            get
            {
                CheckDisposed();
                return m_NavigationEntry;
            }
        }
        #endregion

        #region Events
        public event OperationResultEventHandler<IDocument, TagOperationResultEventArgs>? TagAdded;
        public event OperationResultEventHandler<IDocument, TagOperationResultEventArgs>? TagRemoved;
        #endregion

        #region Constructors
        public Document(OrganizerModel organizer, NavigationTreeDocument treeNode, string title, string content, int itemID) : base(organizer, null, title, content, itemID)
        {
            m_NavigationEntry = treeNode ?? throw new ArgumentNullException(nameof(treeNode));
            m_Tags = new ObservableList<string>();
            m_NavigationEntry.PropertyChanged += NavigationEntry_PropertyChanged;
        }
        #endregion

        #region Interface
        public Task AddTag(string tag)
        {
            CheckDisposed();
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

            return Task.Run(() =>
            {
                try
                {
                    lock (SyncRoot)
                    {
                        if (Tags.Count(x => string.Compare(x, tag, true) == 0) > 0)
                        {
                            dispatcher.BeginInvoke(() => InvokeTagAdded(tag, OperationResultEventArgs.ErrorType.DuplicateValue, "Duplicate tag"));
                            return;
                        }
                    }
                    Organizer.AddDocumentTag(this, tag);
                    dispatcher.BeginInvoke(() =>
                    {
                        lock (SyncRoot)
                            Tags.Add(tag);
                        InvokeTagAdded(tag, OperationResultEventArgs.ErrorType.None, null);
                    });
                }
                catch (Exception e)
                {
                    dispatcher.BeginInvoke(() => InvokeTagAdded(tag, OperationResultEventArgs.ErrorType.InternalError, e.ToString()));
                }
            });
        }

        private void InvokeTagAdded(string tag, OperationResultEventArgs.ErrorType error, string? errorText)
        {
            TagAdded?.Invoke(this, new TagOperationResultEventArgs(tag)
            {
                Error = error,
                ErrorText = errorText
            });
        }

        public Task RemoveTag(string tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

            return Task.Run(() =>
            {
                try
                {
                    lock (SyncRoot)
                    {
                        if (!Tags.Contains(tag))
                        {
                            dispatcher.BeginInvoke(() => InvokeTagAdded(tag, OperationResultEventArgs.ErrorType.InvalidArgument, "Tag not present"));
                            return;
                        }
                    }
                    Organizer.RemoveDocumentTag(this, tag);
                    dispatcher.BeginInvoke(() =>
                    {
                        lock (SyncRoot)
                            Tags.Remove(tag);
                        InvokeTagRemoved(tag, OperationResultEventArgs.ErrorType.None, null);
                    });
                }
                catch (Exception e)
                {
                    dispatcher.BeginInvoke(() => InvokeTagRemoved(tag, OperationResultEventArgs.ErrorType.InternalError, e.ToString()));
                }
            });
        }

        private void InvokeTagRemoved(string tag, OperationResultEventArgs.ErrorType error, string? errorText)
        {
            TagRemoved?.Invoke(this, new TagOperationResultEventArgs(tag)
            {
                Error = error,
                ErrorText = errorText
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
                        if (NavigationEntry.Parent.GetItem(name) != null)
                        {
                            dispatcher.BeginInvoke(() => InvokeRenamed(OperationResultEventArgs.ErrorType.DuplicateValue, "An item with the same title is already present."));
                            return;
                        }
                        oldTitle = Title;
                    }
                    Organizer.ChangeDocumentTitle(NavigationEntry, name);
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

        public override void Dispose()
        {
            base.Dispose();
            m_NavigationEntry = null;
            m_Tags = null;
        }

        protected override void CheckDisposed()
        {
            base.CheckDisposed();
            if (m_NavigationEntry == null || m_Tags == null)
                throw new ObjectDisposedException(nameof(Document));
        }
        #endregion

        #region Methods
        public override StringBuilder GetPath(int len = 0)
        {
            return NavigationEntry.GetPath(len);
        }
        #endregion

        #region EventHandlers
        private void NavigationEntry_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            lock (SyncRoot)
            {
                if (sender == NavigationEntry && e.PropertyName == nameof(Title))
                    Title = NavigationEntry.Title;
            }
        }
        #endregion
    }
}
