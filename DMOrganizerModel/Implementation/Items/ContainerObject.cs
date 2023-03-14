using CSToolbox;
using DMOrganizerModel.Implementation.Organizers;
using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Interface.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.Items
{
    internal class ContainerObject : ContainerItem<IObject>, IObject
    {
        public ContainerObject(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer){}

        // check if all child-objects are valid and can be displayed            !!!
        // (if link to item is not valid or item was deleted - show error) 
        WeakEvent<IItemContainer<IReferenceable>, ItemContainerCurrentContentEventArgs<IReferenceable>> IItemContainer<IReferenceable>.ItemContainerCurrentContent => throw new NotImplementedException();

        WeakEvent<IItemContainer<IReferenceable>, ItemContainerContentChangedEventArgs<IReferenceable>> IItemContainer<IReferenceable>.ItemContainerContentChanged => throw new NotImplementedException();

        public void MakeParentOf(IReferenceable item)
        {
            throw new NotImplementedException();
        }

        public void UpdateContent(IReferenceable newLink)
        {
            throw new NotImplementedException();
        }

        protected override bool DeleteItemInternal()
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<IObject> GetContent()
        {
            throw new NotImplementedException();
        }

        protected override bool HasItem(IObject item)
        {
            throw new NotImplementedException();
        }

        protected override void SetParentInternal(IItemContainerBase parent)
        {
            throw new NotImplementedException();
        }
    }
}
