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


        // create page (page will delete itself)
        // change pages order ?
        // change book name (INamedItem)
        // request book name (INamedItem)
        // request book pages (IContainerItem)
        // content change ivents (IContainerItem)
        void AddPage(IPage page);
        void RemovePage(IPage page);
    }
}
