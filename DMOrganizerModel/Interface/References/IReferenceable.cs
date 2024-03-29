﻿using DMOrganizerModel.Interface.Items;

namespace DMOrganizerModel.Interface.References
{
    public interface IReferenceable : INamedItem
    {
        /// <summary>
        /// Creates a reference to this item
        /// </summary>
        /// <returns>A reference to this item</returns>
        IReference GetReference();
    }
}
