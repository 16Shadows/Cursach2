using System;

namespace DMOrganizerModel.Interface.Items
{
    /// <summary>
    /// Event args for ItemNameChanged of IItem.
    /// </summary>
    public class ItemNameChangedEventArgs : EventArgs
    {
        public enum ResultType
        {
            /// <summary>
            /// The operation was caused by RequestItemNameUpdate, no actual change happened
            /// </summary>
            Requested,
            /// <summary>
            /// The operation was completed successfully
            /// </summary>
            Success,
            /// <summary>
            /// Could not change the name because it would cause name duplication in give context
            /// </summary>
            DuplicateName,
            /// <summary>
            /// Could not change the name because it does not conform to naming rules
            /// </summary>
            InvalidName
        }

        /// <summary>
        /// The new name of the item. May be null if no change has occured.
        /// </summary>
        public string? Name { get; }
        
        /// <summary>
        /// The result of this operation
        /// </summary>
        public ResultType Result { get; }

        /// <summary>
        /// True if the name has actually changed
        /// </summary>
        public bool HasChanged => Result == ResultType.Success;

        public ItemNameChangedEventArgs(string? name, ResultType result = ResultType.Success)
        {
            Name = name;
            Result = result;
        }
    }

    public enum ItemDeletedResult
    {
        /// <summary>
        /// The item was successfully deleted
        /// </summary>
        Success,
        /// <summary>
        /// The item has already been deleted
        /// </summary>
        AlreadyDeleted
    }

    /// <summary>
    /// An interface representing an abstract item entity
    /// </summary>
    public interface IItem
    {
        /// <summary>
        /// Is invoked when the either ChangeItemName or RequestItemNameUpdate is completed.
        /// </summary>
        event TypedEventHandler<IItem, ItemNameChangedEventArgs> ItemNameChanged;

        /// <summary>
        /// Is invoked when the DeleteItem is completed
        /// </summary>
        event TypedEventHandler<IItem, ItemDeletedResult> ItemDeleted;

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
        void DeleteItem();
    }
}
