using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface.Items;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.Items
{
    internal sealed class Category : ContainerItem<IItem>, ICategory
    {
        #region IItem
        public override void DeleteItem()
        {
            CheckDeleted();
            Task.Run(() =>
            {
                bool success = false;
                lock(Lock)
                {
                    success = Query.DeleteCategory(Connection, ItemID);
                    base.DeleteItem();
                }
                InvokeItemDeleted(success ? ItemDeletedResult.Success : ItemDeletedResult.AlreadyDeleted);
            });
        }
        #endregion

        #region IContainerItem
        public override void RequestContainerItemCurrentContent()
        {
            CheckDeleted();

            Task.Run(() =>
            {
                List<IItem> result = new List<IItem>();
                lock (Lock)
                {
                    foreach (int item in Query.GetCategoriesInCategory(Connection, ItemID))
                        result.Add(new Category(item, this, Connection));
                    foreach (int item in Query.GetDocumentsInCategory(Connection, ItemID))
                        result.Add(new Document(item, this, Connection));
                }
                InvokeContainerItemCurrentContent(result);
            });
        }
        #endregion

        #region ICategory
        public void CreateCategory(string name)
        {
            CheckDeleted();
            Task.Run(() =>
            {
                bool isUnique = false;
                IItem item = null;
                lock (Lock)
                {
                    isUnique = Query.HasDuplicateNameInCategory(Connection, name, ItemID);

                    if (isUnique)
                        item = new Category(Query.CreateCategory(Connection, name, ItemID), this, Connection);
                }
                InvokeContainerItemContentChanged(ContainerItemContentChangedEventArgs<IItem>.ChangeType.ItemCreated, item, isUnique ? ContainerItemContentChangedEventArgs<IItem>.ResultType.Success : ContainerItemContentChangedEventArgs<IItem>.ResultType.DuplicateItem);
            });
        }

        public void CreateDocument(string name)
        {
            CheckDeleted();
            Task.Run(() =>
            {
                bool isUnique = false;
                IItem item = null;
                lock (Lock)
                {
                    isUnique = Query.HasDuplicateNameInCategory(Connection, name, ItemID);

                    if (isUnique)
                        item = new Document(Query.CreateDocument(Connection, name, ItemID), this, Connection);
                }
                InvokeContainerItemContentChanged(ContainerItemContentChangedEventArgs<IItem>.ChangeType.ItemCreated, item, isUnique ? ContainerItemContentChangedEventArgs<IItem>.ResultType.Success : ContainerItemContentChangedEventArgs<IItem>.ResultType.DuplicateItem);
            });
        }
        #endregion

        public Category(int itemID, ContainerItemBase? parent, SyncronizedSQLiteConnection connection) : base(itemID, parent, connection) {}

        public override bool SetParent(ContainerItemBase? parent)
        {
            CheckDeleted();
            lock (Lock)
            {
                bool suc = Query.SetCategoryParent(Connection, ItemID, parent?.ItemID);
                Parent = parent;
                return suc;
            }
        }

        protected override bool SetName(string name)
        {
            return Query.SetSectionName(Connection, ItemID, name);
        }

        public override string GetName()
        {
            CheckDeleted();
            lock (Lock)
                return Query.GetCategoryName(Connection, ItemID);
        }

        public override bool HasItem(IItem item)
        {
            return (item is Document doc && Query.CategoryHasDocument(Connection, doc.ItemID, ItemID));
        }

        public override bool HasItemWithName(string name)
        {
            return Query.HasDuplicateNameInCategory(Connection, name, ItemID);
        }
    }
}
