using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DMOrganizerModel.Implementation.Items
{
    internal sealed class Category : ContainerItem<IItem>, ICategory
    {
        #region IItem
        public override void ChangeItemName(string newName)
        {
            CheckDeleted();
            Task.Run(() =>
            {
                //TODO: Validate name
                if (false)
                {
                    InvokeItemNameChanged(null, ItemNameChangedEventArgs.ResultType.InvalidName);
                    return;
                }

                bool isUnique = false;
                lock (Lock)
                {
                    
                    Connection.Read(connection =>
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = $"SELECT ID FROM Category WHERE Title IN (SELECT Title FROM Category WHERE ID={ItemID})";
                            isUnique = command.ExecuteScalar() == null;
                        }
                    });

                    if (isUnique)
                    {
                        Connection.Write(connection =>
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = $"UPDATE Category SET Title=$title WHERE ID={ItemID}";
                                command.Parameters.AddWithValue("$title", newName);
                                command.ExecuteNonQuery();
                            }
                        });
                    }
                }
                if (isUnique)
                    InvokeItemNameChanged(newName, ItemNameChangedEventArgs.ResultType.Success);
                else
                    InvokeItemNameChanged(null, ItemNameChangedEventArgs.ResultType.DuplicateName);         
            });
        }

        public override void DeleteItem()
        {
            CheckDeleted();
            Task.Run(() =>
            {
                bool success = false;
                lock(Lock)
                {
                    Connection.Write(connection =>
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = $"DELETE FROM Category WHERE ID={ItemID}";
                            success = command.ExecuteNonQuery() > 0;
                        }
                    });
                    base.DeleteItem();
                }
                InvokeItemDeleted(success ? ItemDeletedResult.Success : ItemDeletedResult.AlreadyDeleted);
            });
        }

        public override void RequestItemNameUpdate()
        {
            CheckDeleted();
            Task.Run(() =>
            {
                string result = "";
                Connection.Read(connection =>
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT Title FROM Category WHERE ID={ItemID}";
                        using (var reader = command.ExecuteReader())
                        {
                            reader.Read();
                            result = reader.GetString(0);
                        }
                    }
                });
                InvokeItemNameChanged(result, ItemNameChangedEventArgs.ResultType.Requested);
            });
        }
        #endregion

        #region IContainerItem
        public override void AddContainerItem(IItem item)
        {
            CheckDeleted();
        }

        public override void RemoveContainerItem(IItem item)
        {
            CheckDeleted();
        }

        public override void RequestContainerItemCurrentContent()
        {
            CheckDeleted();
        }
        #endregion

        #region ICategory
        public void CreateCategory(string name)
        {
            CheckDeleted();
        }

        public void CreateDocument(string name)
        {
            CheckDeleted();
        }
        #endregion

        public Category(SyncronizedSQLiteConnection connection) : base(connection) {}
    }
}
