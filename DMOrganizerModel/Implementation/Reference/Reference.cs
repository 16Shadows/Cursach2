using System;

using DMOrganizerModel.Implementation.Document;
using DMOrganizerModel.Interface.Document;
using DMOrganizerModel.Interface.Reference;

namespace DMOrganizerModel.Implementation.Reference
{
    internal class Reference : IReference
    {
        #region Properties
        protected SectionBase m_Target;
        public ISection Target => m_Target;
        #endregion

        #region Constructors
        public Reference(SectionBase target)
        {
            m_Target = target ?? throw new ArgumentNullException(nameof(target));
        }
        #endregion

        #region Methods
        public string EncodeAsString()
        { 
            return m_Target.GetPath().ToString();
        }
        #endregion
    }
}
