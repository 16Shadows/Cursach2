using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Items;
using System;

namespace DMOrganizerModel.Implementation.Items
{
    internal class Section : ContainerItem<ISection>, ISection
    {
        public Section(int itemID, ContainerItemBase parent, SyncronizedSQLiteConnection connection) : base(itemID, parent, connection) {}

        #region IContainerItem
        public override void RequestContainerItemCurrentContent()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ISection
        public event TypedEventHandler<ISection, SectionContentChangedEventArgs>? SectionContentChanged;

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

        public override bool SetParent(ContainerItemBase? parent)
        {
            CheckDeleted();
            lock (Lock)
            {
                bool suc = Query.SetSectionParent(Connection, ItemID, parent?.ItemID);
                Parent = parent;
                return suc;
            }
        }

        public override string GetName()
        {
            CheckDeleted();
            lock (Lock)
                return Query.GetSectionName(Connection, ItemID);
        }

        protected override bool SetName(string name)
        {
            CheckDeleted();
            lock (Lock)
                return Query.SetSectionName(Connection, ItemID, name);
        }

        public override bool HasItem(ISection item)
        {
            return (item is Section sec && Query.SectionHasSection(Connection, sec.ItemID, ItemID));
        }

        public override bool HasItemWithName(string name)
        {
            return Query.HasDuplicateNameInSection(Connection, name, ItemID);
        }
    }
}
