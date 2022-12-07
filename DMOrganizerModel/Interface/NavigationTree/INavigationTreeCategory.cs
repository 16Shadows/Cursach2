using System.Collections.Generic;

namespace DMOrganizerModel.Interface.NavigationTree
{
    public interface INavigationTreeCategory : INavigationTreeNodeBase
    {
        List<INavigationTreeNodeBase> Children { get; }
    }
}
