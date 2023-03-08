using System.Data.SQLite;

namespace DMOrganizerModel.Implementation.Utility
{
    internal class SyncronizedSQLiteConnection : SyncronizedConnection<SQLiteConnection>
    {
        public SyncronizedSQLiteConnection(SQLiteConnection connection) : base(connection) {}
    }
}
