using CSToolbox;
using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface.Items;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.Items
{
    internal sealed class Category : NamedContainerItem<IOrganizerItem>, ICategory
    {
        public Category(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer) {}

        #region ICategory
        public WeakEvent<ICategory, CategoryItemCreatedEventArgs> CategoryItemCreated { get; } = new();

        private void InvokeCategoryItemCreated(string name, CategoryItemCreatedEventArgs.ResultType result)
        {
            CategoryItemCreated.Invoke(this, new CategoryItemCreatedEventArgs(name, result));
        }

        public void CreateCategory(string name)
        {
            CheckDeleted();
            Task.Run(() =>
            {
                if (!NamingRules.IsValidName(name))
                {
                    InvokeCategoryItemCreated(name, CategoryItemCreatedEventArgs.ResultType.InvalidName);
                    return;
                }
                bool isUnique = false;
                IOrganizerItem item = null;
                lock (Lock)
                {
                    isUnique = CanHaveItemWithName(name);
                    if (isUnique)
                        item = Organizer.GetCategory(Query.CreateCategory(Organizer.Connection, name, ItemID), this);
                }
                if (isUnique)
                {
                    InvokeCategoryItemCreated(name, CategoryItemCreatedEventArgs.ResultType.Success);
                    InvokeItemContainerContentChanged(item, ItemContainerContentChangedEventArgs<IOrganizerItem>.ChangeType.ItemAdded, ItemContainerContentChangedEventArgs<IOrganizerItem>.ResultType.Success);
                }
                else
                    InvokeCategoryItemCreated(name, CategoryItemCreatedEventArgs.ResultType.DuplicateName);
            });
        }

        public void CreateDocument(string name)
        {
            CheckDeleted();
            Task.Run(() =>
            {
                if (!NamingRules.IsValidName(name))
                {
                    InvokeCategoryItemCreated(name, CategoryItemCreatedEventArgs.ResultType.InvalidName);
                    return;
                }
                bool isUnique = false;
                IOrganizerItem item = null;
                lock (Lock)
                {
                    isUnique = CanHaveItemWithName(name);
                    if (isUnique)
                        item = Organizer.GetDocument(Query.CreateDocument(Organizer.Connection, name, ItemID), this);
                }
                if (isUnique)
                {
                    InvokeCategoryItemCreated(name, CategoryItemCreatedEventArgs.ResultType.Success);
                    InvokeItemContainerContentChanged(item, ItemContainerContentChangedEventArgs<IOrganizerItem>.ChangeType.ItemAdded, ItemContainerContentChangedEventArgs<IOrganizerItem>.ResultType.Success);
                }
                else
                    InvokeCategoryItemCreated(name, CategoryItemCreatedEventArgs.ResultType.DuplicateName);
            });
        }
        #endregion

        public override string GetName()
        {
            return Query.GetCategoryName(Organizer.Connection, ItemID);
        }

        public override void SetName(string name)
        {
            _ = Query.SetCategoryName(Organizer.Connection, ItemID, name);
        }

        protected override IEnumerable<IOrganizerItem> GetContent()
        {
            List<IOrganizerItem> result = new List<IOrganizerItem>();
            foreach (int id in Query.GetCategoriesInCategory(Organizer.Connection, ItemID))
                result.Add(Organizer.GetCategory(id, this));
            foreach (int id in Query.GetDocumentsInCategory(Organizer.Connection, ItemID))
                result.Add(Organizer.GetDocument(id, this));
            return result;
        }

        protected override bool CanBeParentOf(IOrganizerItem item)
        {
            if (item is not INamedItemBase itemTyped)
                throw new ArgumentTypeException(nameof(item), "Unsupported item type.");

            return CanHaveItemWithName(itemTyped.GetName());
        }

        public override bool CanHaveItemWithName(string name)
        {
            return !Query.HasNameInCategory(Organizer.Connection, name, ItemID);
        }

        protected override bool HasItem(IOrganizerItem item)
        {
            if (item is not Item)
                throw new ArgumentTypeException(nameof(item), "Invalid item type.");

            return (item is Document doc && Query.CategoryHasDocument(Organizer.Connection, doc.ItemID, ItemID)) ||
                   (item is Category cat && Query.CategoryHasCategory(Organizer.Connection, cat.ItemID, ItemID));
        }

        protected override bool DeleteItemInternal()
        {
            return Query.DeleteCategory(Organizer.Connection, ItemID);
        }

        protected override void SetParentInternal(IItemContainerBase parent)
        {
            Query.SetCategoryParent(Organizer.Connection, ItemID, (parent as Category)?.ItemID);
        }
    }
}
