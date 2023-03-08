using System;

namespace DMOrganizerModel.Interface.Items
{
    /// <summary>
    /// Event args for ItemNameChanged of IItem.
    /// </summary>
    public class ItemNameChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new name of the item.
        /// </summary>
        public string Name { get; }

        public ItemNameChangedEventArgs(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }

    /// <summary>
    /// An interface representing an abstract item entity
    /// </summary>
    public interface IItem
    {
        /// <summary>
        /// Is invoked when the name of this item changes.
        /// </summary>
        event TypedEventHandler<IItem, ItemNameChangedEventArgs> ItemNameChanged;

        /// <summary>
        /// Requests a name update for this item, causing ItemNameChanged event.
        /// </summary>
        /// <exception cref="InvalidOperationException">Can be thrown if the IItem has already been deleted</exception>
        void RequestItemNameUpdate();

        /// <summary>
        /// Changes the name of this item.
        /// </summary>
        /// <param name="newName">The new name of this item.</param>
        /// <exception cref="InvalidOperationException">Can be thrown if the IItem has already been deleted</exception>
        void ChangeItemName(string newName);

        /// <summary>
        /// Deletes the assosiated entity
        /// </summary>
        /// <exception cref="InvalidOperationException">Can be thrown if the IItem has already been deleted</exception>
        void Delete();
    }
}
