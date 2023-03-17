using CSToolbox.Weak;
using DMOrganizerModel.Implementation.Organizers;
using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Items;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.Items
{
    /// <summary>
    /// A generic implementation of container item
    /// </summary>
    /// <typeparam name="ContentType">The type of items container within</typeparam>
    internal abstract class ContainerItem<ContentType> : Item, IItemContainer<ContentType>, IItemContainerBase where ContentType : IItem
    {
        protected ContainerItem(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer) {}

        #region IItemsContainer
        public WeakEvent<IItemContainer<ContentType>, ItemContainerCurrentContentEventArgs<ContentType>> ItemContainerCurrentContent { get; } = new();
        public WeakEvent<IItemContainer<ContentType>, ItemContainerContentChangedEventArgs<ContentType>> ItemContainerContentChanged { get; } = new();

        protected void InvokeItemContainerCurrentContent(IEnumerable<ContentType> items)
        {
            ItemContainerCurrentContent?.Invoke(this, new ItemContainerCurrentContentEventArgs<ContentType>(items));
        }

        protected void InvokeItemContainerContentChanged(ContentType item, ItemContainerContentChangedEventArgs<ContentType>.ChangeType type, ItemContainerContentChangedEventArgs<ContentType>.ResultType result)
        {
            ItemContainerContentChanged?.Invoke(this, new ItemContainerContentChangedEventArgs<ContentType>(item, type, result));
        }

        public void MakeParentOf(ContentType item)
        {
            if (item is not Item itemBase)
                throw new ArgumentTypeException(nameof(item), "Unsupported item type.");

            Task.Run(() =>
            {
                bool isUnique = false;
                lock (Lock)
                {
                    isUnique = CanBeParentOf(item);
                    if (isUnique)
                        itemBase.SetParent(this);
                }
                InvokeItemContainerContentChanged(item, ItemContainerContentChangedEventArgs<ContentType>.ChangeType.ItemAdded, isUnique ? ItemContainerContentChangedEventArgs<ContentType>.ResultType.Success : ItemContainerContentChangedEventArgs<ContentType>.ResultType.DuplicateItem);
            });
        }

        public void RequestItemContainerCurrentContent()
        {
            Task.Run(() =>
            {
                IEnumerable<ContentType> res = null;
                lock (Lock)
                    res = GetContent();
                InvokeItemContainerCurrentContent(res);
            });
        }
        #endregion

        #region IItemsContainerBase
        protected virtual bool CanBeParentOf(ContentType item) => true;
        public virtual bool CanHaveItemWithName(string name) => true;
        #endregion

        protected abstract IEnumerable<ContentType> GetContent();

        public void OnItemRemoved(IItem item)
        {
            if (item is not ContentType itemTyped)
                throw new ArgumentTypeException(nameof(item), "Invalid item type");

            InvokeItemContainerContentChanged(itemTyped, ItemContainerContentChangedEventArgs<ContentType>.ChangeType.ItemRemoved, ItemContainerContentChangedEventArgs<ContentType>.ResultType.Success);
        }
    }
}
