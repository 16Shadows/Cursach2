using CSToolbox.Weak;
using DMOrganizerModel.Implementation.Organizers;
using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Interface.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSToolbox.Weak;
using System.Net.Mime;
using DMOrganizerModel.Implementation.Utility;

namespace DMOrganizerModel.Implementation.Items
{
    internal class ContainerObject : ContainerItem<IReferenceable>, IObject
    {
        // check if all child-objects are valid and can be displayed            !!!
        // (if link to item is not valid or item was deleted - show error) 
        public ContainerObject(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer){}

        public void UpdateContent(IReference newLink)
        {
            //need to get item by link
            string link = newLink.Encode();
            if (Query.SetObjectLink(Organizer.Connection, ItemID, link))
            {

                InvokeItemContainerContentChanged(oldItem, ItemContainerContentChangedEventArgs<IReferenceable>.ChangeType.ItemRemoved, ItemContainerContentChangedEventArgs<IReferenceable>.ResultType.Success);
                InvokeItemContainerContentChanged(newItem, ItemContainerContentChangedEventArgs<IReferenceable>.ChangeType.ItemAdded, ItemContainerContentChangedEventArgs<IReferenceable>.ResultType.Success);
            }
        }
       
        protected override IEnumerable<IReferenceable> GetContent()
        {
            List<string> items = Query.GetObjectContent(Organizer.Connection, ItemID);
            List<IReference> itemsRef = new List<IReference>();
            foreach (string refString in items)
                itemsRef.Add(Organizer.DecodeReference(refString));

            //need to get objects by references
            InvokeItemContainerCurrentContent(itemsRef);
            return itemsRef;
        }

        protected override bool HasItem(IReferenceable item)
        {
            //gets reference, encode it in string format and returns if object has the same string in db
            string link = item.GetReference().Encode();
            return Query.ObjectHasLink(Organizer.Connection, ItemID, link);
        }
        public void SetLink(IReference link)
        {
            string strlink = link.Encode();
            Query.SetObjectLink(Organizer.Connection, ItemID, strlink);
            // need to get IReferenceable object by link
            InvokeItemContainerContentChanged("!!!", ItemContainerContentChangedEventArgs<IReferenceable>.ChangeType.ItemAdded, ItemContainerContentChangedEventArgs<IReferenceable>.ResultType.Success);
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
