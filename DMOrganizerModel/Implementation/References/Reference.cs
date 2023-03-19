using DMOrganizerModel.Implementation.Items;
using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface.References;
using System;

namespace DMOrganizerModel.Implementation.References
{
    internal class Reference : IReference
    {
        #region IReference
        public IReferenceable Item { get; }
        protected IReferenceInfrastructure ItemTyped { get; }

        public string Encode()
        {
            return ItemTyped.GetPath(0).ToString();
        }
        #endregion

        public Reference(IReferenceable item)
        {
            if (item is not IReferenceInfrastructure itemTyped)
                throw new ArgumentTypeException(nameof(item));
            Item = item ?? throw new ArgumentNullException(nameof(item));
            ItemTyped = itemTyped;
        }
    }
}
