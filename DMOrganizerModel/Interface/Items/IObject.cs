﻿using DMOrganizerModel.Interface.Reference;

namespace DMOrganizerModel.Interface.Items
{
    /// <summary>
    /// Object for containers on pages, can contain links to documents/sections/other linkeable things.
    /// CHANGE TO LINKABLE ITEMS somehow              !!!
    /// </summary>
    public interface IObject: IItem, IItemContainer<IReferenceable>
    {
        // link to object
    }
}
