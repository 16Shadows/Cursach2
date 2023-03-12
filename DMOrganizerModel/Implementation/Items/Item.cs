using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Items;
using System;

namespace DMOrganizerModel.Implementation.Items
{
    internal class Item : IItem
    {
        public Item(int itemID, IItemContainerBase parent, Organizer organizer)
        {
            Organizer = organizer ?? throw new ArgumentNullException(nameof(organizer));
            Parent = parent ?? throw new ArgumentNullException(nameof(organizer));
            ItemID = itemID;
            IsDeleted = false;
            Lock = new object();
        }

        #region IItem
        public event TypedEventHandler<IItem, ItemDeletedResult>? ItemDeleted;

        public virtual void DeleteItem()
        {
            Parent.OnItemRemoved(this);
            IsDeleted = true;
        }
        #endregion

        /// <summary>
        /// The rowid of this item
        /// </summary>
        public int ItemID { get; }

        /// <summary>
        /// The organizer this item belongs to
        /// </summary>
        protected Organizer Organizer { get; }

        /// <summary>
        /// The parent of this item
        /// </summary>
        protected IItemContainerBase Parent { get; private set; }

        /// <summary>
        /// The lock used to syncronize multi-threaded access to this object
        /// </summary>
        protected object Lock { get; }

        /// <summary>
        /// Internal state marker to mark deleted objects since deletion of entity in database doesn't mean that the lifetime of this object ends
        /// </summary>
        private bool IsDeleted { get; set; }

        /// <summary>
        /// A method to check if this object has been deleted
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws an exception if this object has been deleted</exception>
        protected void CheckDeleted()
        {
            if (IsDeleted)
                throw new InvalidOperationException("This item has been deleted.");
        }

        public virtual void SetParent(IItemContainerBase parent)
        {
            Parent.OnItemRemoved(this);
            Parent = parent;
        }
    }
}
