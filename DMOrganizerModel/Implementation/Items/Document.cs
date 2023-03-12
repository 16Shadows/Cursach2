using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Items;

namespace DMOrganizerModel.Implementation.Items
{
    internal sealed class Document : Section, IDocument
    {
        public Document(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer) {}

        #region IDocument
        public event TypedEventHandler<IDocument, DocumentCurrentTagsEventArgs>? DocumentCurrentTags;
        public event TypedEventHandler<IDocument, DocumentTagsChangedEventArgs>? DocumentTagsChanged;

        public void AddDocumentTag(string tag)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveDocumentTag(string tag)
        {
            throw new System.NotImplementedException();
        }

        public void RequestDocumentCurrentTags()
        {
            throw new System.NotImplementedException();
        }
        #endregion

        public override void SetParent(IItemContainerBase parent)
        {
            base.SetParent(parent);
        }
    }
}
