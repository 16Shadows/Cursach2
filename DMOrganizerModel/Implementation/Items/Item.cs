using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Items;
using System;

namespace DMOrganizerModel.Implementation.Items
{
    internal abstract class Item : IItem
    {
        #region IItem
        public event TypedEventHandler<IItem, ItemNameChangedEventArgs>? ItemNameChanged;
        public event TypedEventHandler<IItem, ItemDeletedResult>? ItemDeleted;

        protected void InvokeItemNameChanged(string name, ItemNameChangedEventArgs.ResultType result = ItemNameChangedEventArgs.ResultType.Success)
        {
            ItemNameChanged?.Invoke(this, new ItemNameChangedEventArgs(name));
        }

        protected void InvokeItemDeleted(ItemDeletedResult result)
        {
            ItemDeleted?.Invoke(this, result);
        }

        public virtual void DeleteItem()
        {
            IsDeleted = true;
        }

        public abstract void ChangeItemName(string newName);
        public abstract void RequestItemNameUpdate();
        #endregion

        /// <summary>
        /// Marks whether this item is currently an orphan (is not a part of Organizer or ContainerItem).
        /// Orphan nodes are automatically deleted when their lifetime in the application ends
        /// </summary>
        public bool IsOrphan { get; set; }
        /// <summary>
        /// The rowid of this item
        /// </summary>
        public int ItemID { get; }
        
        /// <summary>
        /// The connection syncronizer to use when accessing the database from this object
        /// </summary>
        protected SyncronizedSQLiteConnection Connection { get; }
        /// <summary>
        /// The lock used to syncronize multi-threaded access to this object
        /// </summary>
        protected object Lock { get; }
        /// <summary>
        /// Internal state marker to mark deleted objects since deletion of entity in database doesn't mean that the lifetime of this object ends
        /// </summary>
        private bool IsDeleted { get; set; }

        public Item(int itemID, SyncronizedSQLiteConnection connection)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection)); 
            ItemID = itemID;
            IsOrphan = false;
            IsDeleted = false;
            Lock = new object();
        }

        ~Item()
        {
            //Delete an orphan entity if its object's lifetime has ended
            if (!IsDeleted && IsOrphan)
                DeleteItem();
        }

        /// <summary>
        /// A method to check if this object has been deleted
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws an exception if this object has been deleted</exception>
        protected void CheckDeleted()
        {
            if (IsDeleted)
                throw new InvalidOperationException("This item has been deleted.");
        }
    }
}
