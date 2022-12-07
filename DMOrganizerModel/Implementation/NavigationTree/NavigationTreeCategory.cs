using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.NavigationTree;

namespace DMOrganizerModel.Implementation.NavigationTree
{
    internal class NavigationTreeCategory : NavigationTreeNodeBase, INavigationTreeCategory
    {
        public IObservableList<INavigationTreeNodeBase> Children => throw new System.NotImplementedException();

        public event OperationResultEventHandler<INavigationTreeCategory, DocumentCreatedEventArgs>? DocumentCreated;
        public event OperationResultEventHandler<INavigationTreeCategory, CategoryCreatedEventArgs>? CategoryCreated;

        public bool CreateCategory(string title)
        {
            throw new System.NotImplementedException();
        }

        public bool CreateDocument(string title)
        {
            throw new System.NotImplementedException();
        }
    }
}
