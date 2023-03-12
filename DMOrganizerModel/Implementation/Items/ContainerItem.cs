using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Items;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.Items
{
    /// <summary>
    /// A generic implementation of container item
    /// </summary>
    /// <typeparam name="ContentType">The type of items container within</typeparam>
    internal abstract class ContainerItem<ContentType> : Item, IContainerItem<ContentType>, IItemContainerBaseTyped<ContentType> where ContentType : IItem
    {
        protected ContainerItem(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer) {}

        #region IItemsContainer
        public event TypedEventHandler<IContainerItem<ContentType>, ItemsContainerCurrentContentEventArgs<ContentType>>? ItemsContainerCurrentContent;
        public event TypedEventHandler<IContainerItem<ContentType>, ItemsContainerContentChangedEventArgs<ContentType>>? ItemsContainerContentChanged;

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
        #endregion

        protected abstract IEnumerable<ContentType> GetContent();
    }
}
