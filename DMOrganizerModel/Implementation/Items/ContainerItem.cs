using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Items;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.Items
{
    /// <summary>
    /// A generic implementation of container item
    /// </summary>
    /// <typeparam name="ContentType">The type of items container within</typeparam>
    internal abstract class ContainerItem<ContentType> : ContainerItemBase, IContainerItem<ContentType> where ContentType : IItem
    {
        #region IContainerItem
        public event TypedEventHandler<IContainerItem<ContentType>, ContainerItemCurrentContentEventArgs<ContentType>>? ContainerItemCurrentContent;

        protected void InvokeContainerItemCurrentContent(List<ContentType> content)
        {
            ContainerItemCurrentContent?.Invoke(this, new ContainerItemCurrentContentEventArgs<ContentType>(content));
        }

        public event TypedEventHandler<IContainerItem<ContentType>, ContainerItemContentChangedEventArgs<ContentType>>? ContainerItemContentChanged;

        protected void InvokeContainerItemContentChanged(ContainerItemContentChangedEventArgs<ContentType>.ChangeType type, ContentType item, ContainerItemContentChangedEventArgs<ContentType>.ResultType result = ContainerItemContentChangedEventArgs<ContentType>.ResultType.Success)
        {
            ContainerItemContentChanged?.Invoke(this, new ContainerItemContentChangedEventArgs<ContentType>(item, type, result));
        }
        
        public abstract void RequestContainerItemCurrentContent();

        public void AddContainerItem(ContentType item)
        {
            CheckDeleted();
            if (item is not Item itemTyped)
                throw new ArgumentTypeException(nameof(item));

            Task.Run(() =>
            {
                bool isUnique;
                lock (Lock)
                {
                    isUnique = HasItemWithName(itemTyped.GetName());
                    if (isUnique)
                        itemTyped.SetParent(this);
                }
                
                InvokeContainerItemContentChanged(ContainerItemContentChangedEventArgs<ContentType>.ChangeType.ItemAdded, item, isUnique ? ContainerItemContentChangedEventArgs<ContentType>.ResultType.Success : ContainerItemContentChangedEventArgs<ContentType>.ResultType.DuplicateItem);
            });
        }

        public void RemoveContainerItem(ContentType item)
        {
            CheckDeleted();
            if (item is not Item itemTyped)
                throw new ArgumentTypeException(nameof(item));

            Task.Run(() =>
            {
                bool hasItem = false;
                lock(Lock)
                    lock (item)
                    {
                        hasItem = HasItem(item);
                        if (hasItem)
                            itemTyped.SetParent(null);
                    }
                InvokeContainerItemContentChanged(ContainerItemContentChangedEventArgs<ContentType>.ChangeType.ItemRemoved, item, hasItem ? ContainerItemContentChangedEventArgs<ContentType>.ResultType.Success : ContainerItemContentChangedEventArgs<ContentType>.ResultType.NoSuchItem);
            });
        }
        #endregion

        public ContainerItem(int itemID, ContainerItemBase parent, SyncronizedSQLiteConnection connection) : base(itemID, parent, connection) {}

        public abstract bool HasItem(ContentType item);
    }
}
