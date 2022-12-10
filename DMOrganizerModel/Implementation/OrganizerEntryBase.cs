using DMOrganizerModel.Implementation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation
{
    internal class OrganizerEntryBase : IDisposable
    {
        private OrganizerModel? m_Organizer;
        public OrganizerModel Organizer
        {
            get
            {
                return m_Organizer ?? throw new ObjectDisposedException(GetType().Name);
            }
        }
        private object? m_SyncRoot;
        protected object SyncRoot
        {
            get
            {
                return m_SyncRoot ?? throw new ObjectDisposedException(GetType().Name);
            }
        }

        protected OrganizerEntryBase(OrganizerModel organizer)
        {
            m_Organizer = organizer ?? throw new ArgumentNullException(nameof(organizer));
            m_SyncRoot = new object();
        }

        public virtual void Dispose()
        {
            m_Organizer = null;
            m_SyncRoot = null;
        }
    }
}
