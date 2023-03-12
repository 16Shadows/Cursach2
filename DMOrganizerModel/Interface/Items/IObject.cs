using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Interface.Items
{
    /// <summary>
    /// Object for containers on pages, can contain links to documents/sections/other linkeable things.
    /// CHANGE TO LINKABLE ITEMS somehow              !!!
    /// </summary>
    public interface IObject: IItem, IItemContainer<ILinkableItem>
    {
        // link to object
    }
}
