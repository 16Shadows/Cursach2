using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Items;
using System;
using System.Threading.Tasks;

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

        public void ChangeItemName(string newName)
        {
            CheckDeleted();

            Task.Run(() =>
            {
                if (NamingRules.IsValidName(newName))
                {
                    InvokeItemNameChanged(null, ItemNameChangedEventArgs.ResultType.InvalidName);
                    return;
                }

                bool isUnique = false;
                lock (Lock)
                {
                    isUnique = !Parent.HasItemWithName(newName);
                    
                    if (isUnique)
                        SetName(newName);
                }
                if (isUnique)
                    InvokeItemNameChanged(newName, ItemNameChangedEventArgs.ResultType.Success);
                else
                    InvokeItemNameChanged(null, ItemNameChangedEventArgs.ResultType.DuplicateName);         
            });
        }
        
        public void RequestItemNameUpdate()
        {
            CheckDeleted();
            Task.Run(() => InvokeItemNameChanged(GetName(), ItemNameChangedEventArgs.ResultType.Requested));
        }
        #endregion

        /// <summary>
        /// The rowid of this item
        /// </summary>
        public int ItemID { get; }
        
        /// <summary>
        /// The rowid of this item's parent
        /// </summary>
        public ContainerItemBase Parent { get; protected set; }

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

        public Item(int itemID, ContainerItemBase parent, SyncronizedSQLiteConnection connection)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection)); 
            ItemID = itemID;
            Parent = parent;
            IsDeleted = false;
            Lock = new object();
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

        /// <summary>
        /// Sets current parent of this item syncronyously
        /// </summary>
        /// <param name="parent">The parent to set</param>
        /// <returns>true on success</returns>
        public abstract bool SetParent(ContainerItemBase? parent);

        /// <summary>
        /// Gets this item's name syncronyously
        /// </summary>
        /// <returns></returns>
        public abstract string GetName();

        protected abstract bool SetName(string name);
    }
}
