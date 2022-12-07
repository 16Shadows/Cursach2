using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.NavigationTree;

namespace DMOrganizerModel.Implementation.NavigationTree
{
    internal class NavigationTreeNodeBase : INavigationTreeNodeBase
    {
        public string Title => throw new System.NotImplementedException();

        public INavigationTreeCategory Parent => throw new System.NotImplementedException();

        public event OperationResultEventHandler<INavigationTreeNodeBase>? Renamed;
        public event OperationResultEventHandler<INavigationTreeNodeBase>? ParentChanged;

        public bool ChangeParent(INavigationTreeCategory newParent)
        {
            throw new System.NotImplementedException();
        }

        public bool Rename(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}
