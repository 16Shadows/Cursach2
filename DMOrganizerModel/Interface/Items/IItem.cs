using CSToolbox.Weak;
using DMOrganizerModel.Interface.Organizer;
using System;

namespace DMOrganizerModel.Interface.Items
{
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

    public interface IItem
    {
        public IOrganizer Organizer { get; }

        /// <summary>
        /// Is invoked when the DeleteItem is completed
        /// </summary>
        WeakEvent<IItem, ItemDeletedResult> ItemDeleted { get; }

        /// <summary>
        /// Deletes the assosiated entity
        /// </summary>
        /// <exception cref="InvalidOperationException">Can be thrown if the IItem has already been deleted</exception>
        void DeleteItem();
    }
}
