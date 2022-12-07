using DMOrganizerModel.Interface.Document;
using DMOrganizerModel.Interface.NavigationTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.NavigationTree
{
    internal class NavigationTreeDocument : NavigationTreeNodeBase, INavigationTreeDocument
    {
        public IDocument Document => throw new NotImplementedException();
    }
}
