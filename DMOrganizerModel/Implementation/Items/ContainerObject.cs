using CSToolbox.Weak;
using DMOrganizerModel.Implementation.Organizers;
using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Interface.References;
using System.Collections.Generic;

namespace DMOrganizerModel.Implementation.Items
{
    internal class ContainerObject : ContainerItem<IReferenceable>, IObject
    {
        public WeakEvent<IObject, ObjectUpdateLinkEventArgs> ObjectUpdateLink { get; } = new();

        private void InvokeObjectUpdateLink(int ID, string link, ObjectUpdateLinkEventArgs.ResultType result)
        {
            ObjectUpdateLink.Invoke(this, new ObjectUpdateLinkEventArgs(link, result));
        }

        // check if all child-objects are valid and can be displayed            !!!
        // (if link to item is not valid or item was deleted - show error) 
        public ContainerObject(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer){}

        public void UpdateContent(IReference newLink)
        {
            string link = newLink.Encode();
            IReferenceable oldItem = null;
            IReferenceable newItem = null;
            if (Query.GetObjectContent(Organizer.Connection, ItemID).Any())
            {
                oldItem = (Organizer.DecodeReferenceInternal(Query.GetObjectContent(Organizer.Connection, ItemID)[0])).Item;
            }
            if (Query.SetObjectLink(Organizer.Connection, ItemID, link))
            {
                if (oldItem != null) InvokeItemContainerContentChanged(oldItem, ItemContainerContentChangedEventArgs<IReferenceable>.ChangeType.ItemRemoved, ItemContainerContentChangedEventArgs<IReferenceable>.ResultType.Success);
                
                newItem = (Organizer.DecodeReferenceInternal(Query.GetObjectContent(Organizer.Connection, ItemID)[0])).Item;
                InvokeItemContainerContentChanged(newItem, ItemContainerContentChangedEventArgs<IReferenceable>.ChangeType.ItemAdded, ItemContainerContentChangedEventArgs<IReferenceable>.ResultType.Success);
                InvokeObjectUpdateLink(ItemID, newItem.GetReference().Encode(), ObjectUpdateLinkEventArgs.ResultType.Success);
            }
            else InvokeObjectUpdateLink(ItemID, newItem.GetReference().Encode(), ObjectUpdateLinkEventArgs.ResultType.IncorrectLink);
        }
        public string GetObjectLink()
        {
            string link = Query.GetObjectLink(Organizer.Connection, ItemID);
            return link;
        }

        protected override IEnumerable<IReferenceable> GetContent()
        {
            List<string> items = Query.GetObjectContent(Organizer.Connection, ItemID);
            List<IReferenceable> result = new List<IReferenceable>();
            foreach (string refString in items)
                result.Add(Organizer.DecodeReferenceInternal(refString).Item); //gets items by reference link
            InvokeItemContainerCurrentContent(result);
            return result;
        }

        public void SetLink(IReference link)
        {
            string strlink = link.Encode();
            Query.SetObjectLink(Organizer.Connection, ItemID, strlink);
            // need to get IReferenceable object by link
            InvokeItemContainerContentChanged((Organizer.DecodeReferenceInternal(Query.GetObjectContent(Organizer.Connection, ItemID)[0])).Item, ItemContainerContentChangedEventArgs<IReferenceable>.ChangeType.ItemAdded, ItemContainerContentChangedEventArgs<IReferenceable>.ResultType.Success);
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
