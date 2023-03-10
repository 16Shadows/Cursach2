using System;
using System.Collections.Generic;

namespace DMOrganizerModel.Interface.Items
{
    /// <summary>
    /// Event args for ContainerItemCurrentContent of IContainerItem.
    /// </summary>
    public class ContainerItemCurrentContentEventArgs<ContentType> : EventArgs where ContentType : IItem
    {
        /// <summary>
        /// The entire current content of container item.
        /// </summary>
        public IEnumerable<ContentType> Content { get; }

        public ContainerItemCurrentContentEventArgs(IEnumerable<ContentType> content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }
    }

    /// <summary>
    /// Event args for ContainerItemContentChanged of IContainerItem.
    /// </summary>
    public class ContainerItemContentChangedEventArgs<ContentType> : EventArgs where ContentType : IItem
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
            DuplicateItem,
            /// <summary>
            /// There is no such item
            /// </summary>
            NoSuchItem
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
            ItemRemoved,
            /// <summary>
            /// An item was created within the container
            /// </summary>
            ItemCreated
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

        public ContainerItemContentChangedEventArgs(ContentType item, ChangeType type, ResultType result = ResultType.Success)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
            Type = type;
            Result = result;
        }
    }

    /// <summary>
    /// An interface representing an abstract item that can contain other items
    /// </summary>
    /// <typeparam name="ContentType">The type of items this item can contain</typeparam>
    public interface IContainerItem<ContentType> : IItem where ContentType : IItem
    {
        /// <summary>
        /// Is invoked when a request for this item's current content is complete.
        /// </summary>
        event TypedEventHandler<IContainerItem<ContentType>, ContainerItemCurrentContentEventArgs<ContentType>> ContainerItemCurrentContent;
        
        /// <summary>
        /// Is invoked when an item is added to or removed from this item.
        /// </summary>
        event TypedEventHandler<IContainerItem<ContentType>, ContainerItemContentChangedEventArgs<ContentType>> ContainerItemContentChanged;


        /// <summary>
        /// Requests item's current content.
        /// </summary>
        /// <exception cref="InvalidOperationException">Can be thrown if the items has already been deleted</exception>
        void RequestContainerItemCurrentContent();
        
        /// <summary>
        /// Adds an item to this IContainerItem.
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <exception cref="InvalidOperationException">Can be thrown if the IContainerItem has already been deleted</exception>
        void AddContainerItem(ContentType item);

        /// <summary>
        /// Removes and item from this IContainerItem.
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <exception cref="InvalidOperationException">Can be thrown if the IContainerItem has already been deleted</exception>
        void RemoveContainerItem(ContentType item);
    }
}
