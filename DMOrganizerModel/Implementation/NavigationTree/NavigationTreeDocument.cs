using DMOrganizerModel.Interface.Document;
using DMOrganizerModel.Interface.NavigationTree;

namespace DMOrganizerModel.Implementation.NavigationTree
{
    internal class NavigationTreeDocument : NavigationTreeNodeBase, INavigationTreeDocument
    {
        public IDocument Document => throw new System.NotImplementedException();
    }
}
