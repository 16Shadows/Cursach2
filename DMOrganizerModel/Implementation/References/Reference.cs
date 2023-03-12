using DMOrganizerModel.Interface.References;

namespace DMOrganizerModel.Implementation.References
{
    internal class Reference : IReference
    {
        #region IReference
        IReferenceable IReference.Item => Item;
        protected IReferenceableBase Item { get; }

        public string Encode()
        {
            throw new System.NotImplementedException();
        }
        #endregion

        public Reference(IReferenceableBase item)
        {
            Item = item;
        }
    }
}
