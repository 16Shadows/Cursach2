using System;

namespace DMOrganizerModel.Interface.NavigationTree
{
    public interface INavigationTreeNodeBase
    {
        string Name { get; set; }
        INavigationTreeCategory Parent { get; set; }
    }
}
