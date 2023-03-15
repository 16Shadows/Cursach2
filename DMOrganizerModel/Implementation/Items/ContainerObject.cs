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

namespace DMOrganizerModel.Implementation.Items
{
    internal class ContainerObject : ContainerItem<IReferenceable>, IObject
    {
        // check if all child-objects are valid and can be displayed            !!!
        // (if link to item is not valid or item was deleted - show error) 
        public ContainerObject(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer){}

        public WeakEvent<IItemContainer<IReferenceable>, ItemContainerCurrentContentEventArgs<IReferenceable>> ItemContainerCurrentContent { get; } = new();

        public WeakEvent<IItemContainer<IReferenceable>, ItemContainerContentChangedEventArgs<IReferenceable>> ItemContainerContentChanged { get; } = new();

        public void UpdateContent(IReference newLink)
        {
            throw new NotImplementedException();
        }
       
        protected override IEnumerable<IReferenceable> GetContent()
        {
            throw new NotImplementedException();
        }

        protected override bool HasItem(IReferenceable item)
        {
            throw new NotImplementedException();
        }

        protected override void SetParentInternal(IItemContainerBase parent)
        {
            throw new NotImplementedException();
        }
        protected override bool DeleteItemInternal()
        {
            throw new NotImplementedException();
        }

    }
}
