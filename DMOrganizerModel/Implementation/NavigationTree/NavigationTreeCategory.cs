using DMOrganizerModel.Interface.NavigationTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.NavigationTree
{
    internal class NavigationTreeCategory : NavigationTreeNodeBase, INavigationTreeCategory
    {
        public List<INavigationTreeNodeBase> Children => throw new NotImplementedException();
    }
}
