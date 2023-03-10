using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.Items
{
    internal sealed class Document : Section, IDocument
    {
        public Document(int itemID, ContainerItemBase parent, SyncronizedSQLiteConnection connection) : base(itemID, parent, connection) {}

        public event TypedEventHandler<IDocument, DocumentCurrentTagsEventArgs>? DocumentCurrentTags;
        public event TypedEventHandler<IDocument, DocumentTagsChangedEventArgs>? DocumentTagsChanged;

        public void AddDocumentTag(string tag)
        {
            throw new NotImplementedException();
        }

        public void RemoveDocumentTag(string tag)
        {
            throw new NotImplementedException();
        }

        public void RequestDocumentCurrentTags()
        {
            throw new NotImplementedException();
        }
    }
}
