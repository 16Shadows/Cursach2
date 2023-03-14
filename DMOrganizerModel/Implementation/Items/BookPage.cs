using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Implementation.Organizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.Items
{
    internal class BookPage: NamedContainerItem<IObjectContainer>, IPage
    {
        // IPage
        public BookPage(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer) { }

        public void AddContainer()
        {
            throw new NotImplementedException();
        }

        // NamedContainerItem
        public override string GetName()
        {
            throw new NotImplementedException();
        }

        public void RequestPagePosition()
        {
            throw new NotImplementedException();
        }

        public override void SetName(string name)
        {
            throw new NotImplementedException();
        }

        protected override bool DeleteItemInternal()
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<IObjectContainer> GetContent()
        {
            throw new NotImplementedException();
        }

        protected override bool HasItem(IObjectContainer item)
        {
            throw new NotImplementedException();
        }

        protected override void SetParentInternal(IItemContainerBase parent)
        {
            throw new NotImplementedException();
        }
    }
}
