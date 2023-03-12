﻿using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Items;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.Items
{
    internal sealed class Document : Section, IDocument
    {
        public Document(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer) {}

        #region IDocument
        public event TypedEventHandler<IDocument, DocumentCurrentTagsEventArgs>? DocumentCurrentTags;
        public event TypedEventHandler<IDocument, DocumentTagsChangedEventArgs>? DocumentTagsChanged;

        private void InvokeDocumentCurrentTags(IEnumerable<string> items)
        {
            DocumentCurrentTags?.Invoke(this, new DocumentCurrentTagsEventArgs(items));
        }

        private void InvokeDocumentTagsChanged(string tag, DocumentTagsChangedEventArgs.ChangeType type, DocumentTagsChangedEventArgs.ResultType result)
        {
            DocumentTagsChanged?.Invoke(this, new DocumentTagsChangedEventArgs(tag, type, result));
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
    }
}
