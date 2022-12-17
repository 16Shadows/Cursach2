using DMOrganizerModel.Implementation.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Xps.Serialization;

namespace DMOrganizerModel.Implementation
{
    internal class OrganizerEntryBase : IDisposable, INotifyPropertyChanged
    {
        #region Properties
        private OrganizerModel? m_Organizer;
        public OrganizerModel Organizer
        {
            get
            {
                CheckDisposed();
                return m_Organizer;
            }
        }
        private object? m_SyncRoot;
        protected object SyncRoot
        {
            get
            {
                CheckDisposed();
                return m_SyncRoot;
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void InvokePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Constructors
        protected OrganizerEntryBase(OrganizerModel organizer)
        {
            m_Organizer = organizer ?? throw new ArgumentNullException(nameof(organizer));
            m_SyncRoot = new object();
        }
        #endregion

        #region Interfaces
        protected virtual void CheckDisposed()
        {
            if (m_Organizer == null || m_SyncRoot == null)
                throw new ObjectDisposedException(GetType().Name);
        }

        public virtual void Dispose()
        {
            CheckDisposed();
            m_Organizer = null;
            m_SyncRoot = null;
        }
        #endregion
    }
}
