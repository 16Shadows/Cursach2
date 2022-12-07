using DMOrganizerModel.Implementation.Document;
using DMOrganizerModel.Interface.Document;
using DMOrganizerModel.Interface.Reference;

namespace DMOrganizerModel.Implementation.Reference
{
    internal class Reference : IReference
    {
        #region Data
        protected Section m_Target;
        public ISection Target => m_Target;
        #endregion

        #region Constructors
        public Reference(Section target)
        {
            m_Target = target;
        }
        #endregion

        #region Methods
        public string EncodeAsString()
        { 
            return "";
        }
        #endregion
    }
}
