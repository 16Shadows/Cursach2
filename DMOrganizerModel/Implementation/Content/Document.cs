using System;
using System.Text;

using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Content;
using DMOrganizerModel.Implementation.NavigationTree;
using DMOrganizerModel.Implementation.Model;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.Content
{
    internal sealed class Document : SectionBase, IDocument
    {
        #region Properties
        public ObservableList<string> Tags { get; }
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
        public Document(OrganizerModel organizer, NavigationTreeDocument treeNode, string title, string content, int itemID) : base(organizer, title, content, itemID)
        {
            m_NavigationEntry = treeNode ?? throw new ArgumentNullException(nameof(treeNode));
            Tags = new ObservableList<string>();
            m_NavigationEntry.Renamed += NavigationEntry_Renamed;
        }
        #endregion

        #region Interface
        public Task AddTag(string tag)
        {
            CheckDisposed();
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));
            else if (Tags.Contains(tag))
                throw new ArgumentException("Duplicate tag", nameof(tag));

            return Task.Run(() =>
            {
                lock (SyncRoot)
                {
                    try
                    {
                        Organizer.AddDocumentTag(this, tag);
                        Tags.Add(tag);
                        InvokeTagAdded(tag, OperationResultEventArgs.ErrorType.None, null);
                    }
                    catch (Exception e)
                    {
                        InvokeTagAdded(tag, OperationResultEventArgs.ErrorType.InternalError, e.Message);
                    }
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
            else if (!Tags.Contains(tag))
                throw new ArgumentException("Tag not present", nameof(tag));

            return Task.Run(() =>
            {
                lock (SyncRoot)
                {
                    try
                    {
                        Organizer.RemoveDocumentTag(this, tag);
                        Tags.Remove(tag);
                        InvokeTagRemoved(tag, OperationResultEventArgs.ErrorType.None, null);
                    }
                    catch (Exception e)
                    {
                        InvokeTagRemoved(tag, OperationResultEventArgs.ErrorType.InternalError, e.Message);
                    }
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
            return NavigationEntry.Rename(name);
        }
        #endregion

        #region Methods
        public override StringBuilder GetPath(int len = 0)
        {
            return NavigationEntry.GetPath(len);
        }
        #endregion

        #region EventHandlers
        private void NavigationEntry_Renamed(Interface.NavigationTree.INavigationTreeNodeBase sender, OperationResultEventArgs e)
        {
            if (e.Error == OperationResultEventArgs.ErrorType.None)
                Title = sender.Title;
            
            InvokeRenamed(e.Error, e.ErrorText);
        }
        #endregion
    }
}
