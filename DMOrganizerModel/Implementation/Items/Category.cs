using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Items;
using System.Collections.Generic;

namespace DMOrganizerModel.Implementation.Items
{
    internal sealed class Category : NamedContainerItem<IOrganizerItem>, ICategory
    {
        public Category(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer) {}

        #region ICategory
        public event TypedEventHandler<ICategory, CategoryItemCreatedEventArgs>? CategoryItemCreated;

        public void CreateCategory(string name)
        {
            throw new System.NotImplementedException();
        }

        public void CreateDocument(string name)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        public override string GetName()
        {
            throw new System.NotImplementedException();
        }

        public override void SetName(string name)
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<IOrganizerItem> GetContent()
        {
            throw new System.NotImplementedException();
        }

        public override bool CanBeParentOf(IOrganizerItem item)
        {
            return base.CanBeParentOf(item);
        }

        public override bool CanHaveItemWithName(string name)
        {
            return base.CanHaveItemWithName(name);
        }

        public override void SetParent(IItemContainerBase parent)
        {
            base.SetParent(parent);
        }

        public override void DeleteItem()
        {
            base.DeleteItem();
        }
    }
}
