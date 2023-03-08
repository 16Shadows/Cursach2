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
        public List<ContentType> Content { get; }

        public ContainerItemCurrentContentEventArgs(List<ContentType> content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }
    }

    /// <summary>
    /// Event args for ContainerItemContentChanged of IContainerItem.
    /// </summary>
    public class ContainerItemContentChangedEventArgs<ContentType> : EventArgs where ContentType : IItem
    {
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

        public ContainerItemContentChangedEventArgs(ContentType item, ChangeType type)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
            Type = type;
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
        /// <exception cref="ArgumentException">Can be throw if the IContainerItem already has the item</exception>
        /// <exception cref="InvalidOperationException">Can be thrown if the IContainerItem has already been deleted</exception>
        void AddContainerItemItem(ContentType item);

        /// <summary>
        /// Removes and item from this IContainerItem.
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <exception cref="ArgumentException">Can be throw if the IContainerItem doesn't have the item</exception>
        /// <exception cref="InvalidOperationException">Can be thrown if the IContainerItem has already been deleted</exception>
        void RemoveContainerItemItem(ContentType item);
    }
}
