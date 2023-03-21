using CSToolbox.Weak;
using DMOrganizerModel.Implementation.Organizers;
using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Interface.References;
using System.Collections.Generic;

namespace DMOrganizerModel.Implementation.Items
{
    internal class ContainerObject : Item, IObject
    {
        public WeakEvent<IObject, ObjectUpdateLinkEventArgs> ObjectUpdateLink { get; } = new();
        public WeakEvent<IObject, ObjectCurrentContentEventArgs> ObjectCurrentContent { get; } = new();

        public void InvokeObjectUpdateLink(int ID, string link, ObjectUpdateLinkEventArgs.ResultType result)
        {
            ObjectUpdateLink.Invoke(this, new ObjectUpdateLinkEventArgs(link, result));
        }
        public void InvokeObjectCurrentContent(int ID, string link)
        {
            ObjectCurrentContent.Invoke(this, new ObjectCurrentContentEventArgs(link));
        }
        public ContainerObject(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer){}

        public void UpdateContent(IReference newLink)
        {
            string link = newLink.Encode();
            IReferenceable newItem = null;
            if (Query.SetObjectLink(Organizer.Connection, ItemID, link))
            {
               newItem = (Organizer.DecodeReferenceInternal(Query.GetObjectContent(Organizer.Connection, ItemID)[0])).Item;
               InvokeObjectUpdateLink(ItemID, newItem.GetReference().Encode(), ObjectUpdateLinkEventArgs.ResultType.Success);
            }
            else InvokeObjectUpdateLink(ItemID, newItem.GetReference().Encode(), ObjectUpdateLinkEventArgs.ResultType.IncorrectLink);
        }
        public string GetObjectLink()
        {
            string link = Query.GetObjectLink(Organizer.Connection, ItemID);
            if (Organizer.DecodeReferenceInternal(link) == null)
            {
                return null;
            }
            else
            {
                InvokeObjectCurrentContent(ItemID, link);
                return link;
            }
            
        }
        public IReference GetReferenceByLink(string link)
        {
            return Organizer.DecodeReferenceInternal(link);
        }
        public void SetLink(IReference link)
        {
            string strlink = link.Encode();
            Query.SetObjectLink(Organizer.Connection, ItemID, strlink);
            // need to get IReferenceable object by link
            InvokeObjectUpdateLink(ItemID, strlink, ObjectUpdateLinkEventArgs.ResultType.Success);
        }
        protected override void SetParentInternal(IItemContainerBase parent)
        {
            if (parent is not ObjectContainer) throw new ArgumentTypeException(nameof(parent), "Unsupported object parent type.");
            else Query.SetObjectParent(Organizer.Connection, ItemID, (parent as ObjectContainer).ItemID);
        }
        protected override bool DeleteItemInternal()
        {
            return Query.DeleteBook(Organizer.Connection, ItemID);
        }
    }
}
