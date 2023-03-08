using System;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading;

namespace DMOrganizerModel.Implementation.Utility
{
    /// <summary>
    /// Implements a wrapper around connection to syncronize DB writes.
    /// Does not manage the connection's lifetime.
    /// </summary>
    /// <typeparam name="ConnectionType">The connection type to use</typeparam>
    internal class SyncronizedConnection<ConnectionType> where ConnectionType : DbConnection
    {
        private ReaderWriterLockSlim Lock { get; }
        private ConnectionType Connection { get; }

        public SyncronizedConnection(ConnectionType connection)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            Lock = new ReaderWriterLockSlim();
        }

        ~SyncronizedConnection()
        {
            Lock.Dispose();
        }

        /// <summary>
        /// Perform an operation which consists only of DB reads
        /// </summary>
        /// <param name="operation">The operation to perform</param>
        public void Read(Action<ConnectionType> operation)
        {
            Lock.EnterReadLock();
            try
            {
                operation(Connection);
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Perform an operation which involves at least one write
        /// </summary>
        /// <param name="operation">The operation to perform</param>
        public void Write(Action<ConnectionType> operation)
        {
            Lock.EnterWriteLock();
            try
            {
                operation(Connection);
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }
    }
}
