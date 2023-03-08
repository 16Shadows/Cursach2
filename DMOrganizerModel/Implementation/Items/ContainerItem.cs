using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Items;
using System.Collections.Generic;

namespace DMOrganizerModel.Implementation.Items
{
    internal abstract class ContainerItem<ContentType> : Item, IContainerItem<ContentType> where ContentType : IItem
    {
        #region IContainerItem
        public event TypedEventHandler<IContainerItem<ContentType>, ContainerItemCurrentContentEventArgs<ContentType>>? ContainerItemCurrentContent;

        protected void InvokeContainerItemCurrentContent(List<ContentType> content)
        {
            ContainerItemCurrentContent?.Invoke(this, new ContainerItemCurrentContentEventArgs<ContentType>(content));
        }

        public event TypedEventHandler<IContainerItem<ContentType>, ContainerItemContentChangedEventArgs<ContentType>>? ContainerItemContentChanged;

        protected void InvokeContainerItemContentChanged(ContainerItemContentChangedEventArgs<ContentType>.ChangeType type, ContentType item)
        {
            ContainerItemContentChanged?.Invoke(this, new ContainerItemContentChangedEventArgs<ContentType>(item, type));
        }

        public abstract void AddContainerItem(ContentType item);
        public abstract void RemoveContainerItem(ContentType item);
        public abstract void RequestContainerItemCurrentContent();
        #endregion

        public ContainerItem(SyncronizedSQLiteConnection connection) : base(connection) {}
    }
}
