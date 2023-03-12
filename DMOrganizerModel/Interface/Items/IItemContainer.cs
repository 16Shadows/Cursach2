using System;
using System.Collections.Generic;

namespace DMOrganizerModel.Interface.Items
{
    /// <summary>
    /// Event args for ContainerItemCurrentContent of IContainerItem.
    /// </summary>
    public class ItemsContainerCurrentContentEventArgs<ContentType> : EventArgs where ContentType : IItem
    {
        /// <summary>
        /// The entire current content of container item.
        /// </summary>
        public IEnumerable<ContentType> Content { get; }

        public ItemsContainerCurrentContentEventArgs(IEnumerable<ContentType> content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }
    }

    /// <summary>
    /// Event args for ContainerItemContentChanged of IContainerItem.
    /// </summary>
    public class ItemsContainerContentChangedEventArgs<ContentType> : EventArgs where ContentType : IItem
    {
        public enum ResultType
        {
            /// <summary>
            /// The operation was completed successfully
            /// </summary>
            Success,
            /// <summary>
            /// There already is an item with the same name
            /// </summary>
            DuplicateItem
        }

        /// <summary>
        /// The type of change this event represents.
        /// </summary>
        public enum ChangeType
        {
            /// <summary>
            /// An item was added to the IContainerItem.
            /// </summary>
            ItemAdded,
            /// <summary>
            /// An item was removed from this IContainerItem.
            /// </summary>
            ItemRemoved
        }

        /// <summary>
        /// The item that was added or removed.
        /// </summary>
        public ContentType Item { get; }

        /// <summary>
        /// The type of change this event represents.
        /// </summary>
        public ChangeType Type { get; }

        /// <summary>
        /// The result of the operation
        /// </summary>
        public ResultType Result { get; }

        /// <summary>
        /// True if the content has actually changed
        /// </summary>
        public bool HasChanged => Result == ResultType.Success;

        public ItemsContainerContentChangedEventArgs(ContentType item, ChangeType type, ResultType result = ResultType.Success)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
            Type = type;
            Result = result;
        }
    }

    public interface IItemContainer<ContentType> where ContentType : IItem
    {
        /// <summary>
        /// Is invoked when a request for this item's current content is complete.
        /// </summary>
        event TypedEventHandler<IItemContainer<ContentType>, ItemsContainerCurrentContentEventArgs<ContentType>> ItemsContainerCurrentContent;
        
        /// <summary>
        /// Is invoked when an item is added to or removed from this item.
        /// </summary>
        event TypedEventHandler<IItemContainer<ContentType>, ItemsContainerContentChangedEventArgs<ContentType>> ItemsContainerContentChanged;

        /// <summary>
        /// Requests item's current content.
        /// </summary>
        /// <exception cref="InvalidOperationException">Can be thrown if the items has already been deleted</exception>
        void RequestOrganizerItemsContainerCurrentContent();

        /// <summary>
        /// Makes this container the parent of the item
        /// </summary>
        /// <param name="item">The item to make this category parent of</param>
        void MakeParentOf(ContentType item);
    }
}
