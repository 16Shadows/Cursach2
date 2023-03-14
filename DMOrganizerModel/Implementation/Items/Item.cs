using CSToolbox;
using DMOrganizerModel.Implementation.Organizers;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Items;
using System;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.Items
{
    internal abstract class Item : IItem
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
        public WeakEvent<IItem, ItemDeletedResult> ItemDeleted { get; } = new();

        protected void InvokeItemDeleted(ItemDeletedResult result)
        {
            ItemDeleted.Invoke(this, result);
        }

        public void DeleteItem()
        {
            CheckDeleted();
            Task.Run(() =>
            {
                bool deleted = false;
                lock(Lock)
                {
                    deleted = DeleteItemInternal();
                    if (deleted)
                    {
                        Parent.OnItemRemoved(this);
                        IsDeleted = true;
                    }
                }
                InvokeItemDeleted(deleted ? ItemDeletedResult.Success : ItemDeletedResult.AlreadyDeleted);
            });
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

        public void SetParent(IItemContainerBase parent)
        {
            SetParentInternal(parent);
            Parent.OnItemRemoved(this);
            if (Parent is IItem oldItem)
                oldItem.ItemDeleted.Unsubscribe(ParentDeleted);
            Parent = parent;
            if (Parent is IItem newItem)
                newItem.ItemDeleted.Subscribe(ParentDeleted);
        }

        private void ParentDeleted(IItem sender, ItemDeletedResult result)
        {
            IsDeleted = true;
        }

        /// <summary>
        /// Should set parent on actual implementation
        /// </summary>
        /// <param name="parent">The parent to set</param>
        protected abstract void SetParentInternal(IItemContainerBase parent);
        protected abstract bool DeleteItemInternal();
        
    }
}
