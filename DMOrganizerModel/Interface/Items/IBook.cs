using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Interface.Items
{
    /// <summary>
    /// Book, container for pages.
    /// </summary>
    public interface IBook : INamedItem, IItemContainer<IPage>
    {
        //Name, ID of parent category

        // create page
        //change pages order
        //
    }
}
