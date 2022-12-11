using DMOrganizerModel.Implementation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Xps.Serialization;

namespace DMOrganizerModel.Implementation
{
    internal class OrganizerEntryBase : IDisposable
    {
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

        protected OrganizerEntryBase(OrganizerModel organizer)
        {
            m_Organizer = organizer ?? throw new ArgumentNullException(nameof(organizer));
            m_SyncRoot = new object();
        }

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
    }
}
