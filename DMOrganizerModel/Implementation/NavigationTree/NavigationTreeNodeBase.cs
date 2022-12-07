using DMOrganizerModel.Interface.NavigationTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.NavigationTree
{
    internal class NavigationTreeNodeBase : INavigationTreeNodeBase
    {
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public INavigationTreeCategory Parent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
