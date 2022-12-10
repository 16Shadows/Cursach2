using DMOrganizerModel.Interface.NavigationTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.NavigationTree
{
    internal interface INavigationTreeRootInternal : INavigationTreeRoot
    {
        /// <summary>
        /// Is used to efficently construct path to this instance
        /// Specific implementation depends on the object (Section or Document)
        /// </summary>
        /// <param name="len">The number of characters required by previous steps of the algorithm to construct full path</param>
        /// <returns></returns>
        public abstract StringBuilder GetPath(int len = 0);
    }
}
