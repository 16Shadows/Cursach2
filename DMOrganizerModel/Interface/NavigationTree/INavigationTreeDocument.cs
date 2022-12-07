using DMOrganizerModel.Interface.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Interface.NavigationTree
{
    public interface INavigationTreeDocument : INavigationTreeNodeBase
    {
        IDocument Document { get; }
    }
}
