using System;

using DMOrganizerModel.Implementation.Content;
using DMOrganizerModel.Interface.Content;
using DMOrganizerModel.Interface.Reference;

namespace DMOrganizerModel.Implementation.Reference
{
    internal class Reference : IReference
    {
        #region Properties
        protected ItemBase m_Target;
        public IItem Target => m_Target;
        #endregion

        #region Constructors
        public Reference(ItemBase target)
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
