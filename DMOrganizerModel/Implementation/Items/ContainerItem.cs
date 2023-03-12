using DMOrganizerModel.Implementation.Model;
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
    internal abstract class ContainerItem<ContentType> : Item, IItemContainerBaseTyped<ContentType> where ContentType : IItem
    {
        protected ContainerItem(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer) {}

        #region IItemsContainer
        public event TypedEventHandler<IItemContainer<ContentType>, ItemsContainerCurrentContentEventArgs<ContentType>>? ItemsContainerCurrentContent;
        public event TypedEventHandler<IItemContainer<ContentType>, ItemsContainerContentChangedEventArgs<ContentType>>? ItemsContainerContentChanged;

        protected void InvokeItemsContainerCurrentContent(IEnumerable<ContentType> items)
        {
            ItemsContainerCurrentContent?.Invoke(this, new ItemsContainerCurrentContentEventArgs<ContentType>(items));
        }

        protected void InvokeItemsContainerContentChanged(ContentType item, ItemsContainerContentChangedEventArgs<ContentType>.ChangeType type, ItemsContainerContentChangedEventArgs<ContentType>.ResultType result)
        {
            ItemsContainerContentChanged?.Invoke(this, new ItemsContainerContentChangedEventArgs<ContentType>(item, type, result));
        }

        public void MakeParentOf(ContentType item)
        {
            if (item is not ContentType itemTyped || item is not Item itemBase)
                throw new ArgumentTypeException(nameof(item), "Unsupported item type.");

            Task.Run(() =>
            {
                bool isUnique = false;
                lock (Lock)
                {
                    isUnique = CanBeParentOf(itemTyped);
                    if (isUnique)
                        itemBase.SetParent(this);
                }
                InvokeItemsContainerContentChanged(item, ItemsContainerContentChangedEventArgs<ContentType>.ChangeType.ItemAdded, isUnique ? ItemsContainerContentChangedEventArgs<ContentType>.ResultType.Success : ItemsContainerContentChangedEventArgs<ContentType>.ResultType.DuplicateItem);
            });
        }

        public void RequestOrganizerItemsContainerCurrentContent()
        {
            Task.Run(() =>
            {
                IEnumerable<ContentType> res = null;
                lock (Lock)
                    res = GetContent();
                InvokeItemsContainerCurrentContent(res);
            });
        }
        #endregion

        #region IItemsContainerBase
        public virtual bool CanBeParentOf(ContentType item) => true;
        public virtual bool CanHaveItemWithName(string name) => true;
        public abstract bool HasItem(ContentType item);
        #endregion

        protected abstract IEnumerable<ContentType> GetContent();

        public void OnItemRemoved(IItem item)
        {
            if (item is not ContentType itemTyped)
                throw new ArgumentTypeException(nameof(item), "Invalid item type");
            else if (!HasItem(itemTyped))
                throw new ArgumentException("This container does not have such item.", nameof(item));

            InvokeItemsContainerContentChanged(itemTyped, ItemsContainerContentChangedEventArgs<ContentType>.ChangeType.ItemRemoved, ItemsContainerContentChangedEventArgs<ContentType>.ResultType.Success);
        }
    }
}
