using CSToolbox.Weak;
using DMOrganizerModel.Implementation.Organizers;
using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface.Items;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.Items
{
    internal sealed class Document : Section, IDocument
    {
        public Document(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer) {}

        #region IDocument
        public WeakEvent<IDocument, DocumentCurrentTagsEventArgs> DocumentCurrentTags { get; } = new();
        public WeakEvent<IDocument, DocumentTagsChangedEventArgs> DocumentTagsChanged { get; } = new();

        private void InvokeDocumentCurrentTags(IEnumerable<string> items)
        { 
            DocumentCurrentTags.Invoke(this, new DocumentCurrentTagsEventArgs(items));
        }

        private void InvokeDocumentTagsChanged(string tag, DocumentTagsChangedEventArgs.ChangeType type, DocumentTagsChangedEventArgs.ResultType result)
        {
            DocumentTagsChanged.Invoke(this, new DocumentTagsChangedEventArgs(tag, type, result));
        }

        public void AddDocumentTag(string tag)
        {
            CheckDeleted();
            Task.Run(() =>
            {
                if (!NamingRules.IsValidTag(tag))
                {
                    InvokeDocumentTagsChanged(tag, DocumentTagsChangedEventArgs.ChangeType.TagAdded, DocumentTagsChangedEventArgs.ResultType.InvalidTag);
                    return;
                }

                bool isUnique = false;
                lock (Lock)
                {
                    isUnique = !Query.DocumentHasTag(Organizer.Connection, ItemID, tag);
                    if (isUnique)
                        Query.AddDocumentTag(Organizer.Connection, ItemID, tag);
                }
                InvokeDocumentTagsChanged(tag, DocumentTagsChangedEventArgs.ChangeType.TagAdded, isUnique ? DocumentTagsChangedEventArgs.ResultType.Success : DocumentTagsChangedEventArgs.ResultType.InvalidTag);
            }); 
        }

        public void RemoveDocumentTag(string tag)
        {
            CheckDeleted();
            Task.Run(() =>
            {
                bool hasTag = false;
                lock (Lock)
                {
                    hasTag = Query.DocumentHasTag(Organizer.Connection, ItemID, tag);
                    if (hasTag)
                        Query.RemoveDocumentTag(Organizer.Connection, ItemID, tag);
                }
                InvokeDocumentTagsChanged(tag, DocumentTagsChangedEventArgs.ChangeType.TagRemoved, hasTag ? DocumentTagsChangedEventArgs.ResultType.Success : DocumentTagsChangedEventArgs.ResultType.NoSuchTag);
            }); 
        }

        public void RequestDocumentCurrentTags()
        {
            CheckDeleted();
            Task.Run(() => InvokeDocumentCurrentTags(Query.GetDocumentTags(Organizer.Connection, ItemID)));
        }
        #endregion

        protected override void SetParentInternal(IItemContainerBase parent)
        {
            Query.SetDocumentParent(Organizer.Connection, ItemID, (parent as Category).ItemID);
        }

        public override StringBuilder GetPath(int len)
        {
            if (Parent is Category cat)
                return cat.GetPath(len + CachedName.Length + 1).Append('$').Append(CachedName);
            return (new StringBuilder(CachedName.Length+1)).Append('$').Append(CachedName);
        }
    }
}
