using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Items;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DMOrganizerModel.Implementation.Model;

namespace DMOrganizerModel.Implementation.Items
{
    internal class Section : NamedContainerItem<ISection>, ISection
    {
        public Section(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer) {}

        #region ISection
        public event TypedEventHandler<ISection, SectionContentChangedEventArgs>? SectionContentChanged;
        public event TypedEventHandler<ISection, SectionItemCreatedEventArgs>? SectionItemCreated;

        public void CreateSection(string name)
        {
            throw new NotImplementedException();
        }

        public void RequestSectionContentUpdate()
        {
            throw new NotImplementedException();
        }

        public void UpdateContent(string content)
        {
            throw new NotImplementedException();
        }
        #endregion

        public override string GetName()
        {
            throw new NotImplementedException();
        }

        public override void SetName(string name)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<ISection> GetContent()
        {
            throw new NotImplementedException();
        }

        public override bool CanBeParentOf(ISection item)
        {
            return base.CanBeParentOf(item);
        }

        public override bool CanHaveItemWithName(string name)
        {
            return base.CanHaveItemWithName(name);
        }

        public override void DeleteItem()
        {
            base.DeleteItem();
        }

        public override void SetParent(IItemContainerBase parent)
        {
            base.SetParent(parent);
        }

        public override bool HasItem(ISection item)
        {
            throw new NotImplementedException();
        }
    }
}
