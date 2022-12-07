using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Document;
using DMOrganizerModel.Interface.NavigationTree;
using System.Text;

namespace DMOrganizerModel.Implementation.NavigationTree
{
    internal class NavigationTreeCategory : NavigationTreeNodeBase, INavigationTreeCategory
    {
        public IObservableList<INavigationTreeNodeBase> Children => throw new System.NotImplementedException();

        IObservableCollection<INavigationTreeNodeBase> INavigationTreeCategory.Children => throw new System.NotImplementedException();

        public event OperationResultEventHandler<INavigationTreeCategory, DocumentCreatedEventArgs>? DocumentCreated;
        public event OperationResultEventHandler<INavigationTreeCategory, CategoryCreatedEventArgs>? CategoryCreated;
        public event OperationResultEventHandler<INavigationTreeCategory, DocumentDeletedEventArgs>? DocumentDeleted;
        public event OperationResultEventHandler<INavigationTreeCategory, DocumentDeletedEventArgs>? CategoryDeleted;

        public bool CreateCategory(string title)
        {
            throw new System.NotImplementedException();
        }

        public bool CreateDocument(string title)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteCategory(INavigationTreeCategory category)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteDocument(IDocument document)
        {
            throw new System.NotImplementedException();
        }

        public override StringBuilder GetPath(int len = 0)
        {
            //Dummy, replace with actual pathbuilding
            return new StringBuilder();
        }
    }
}
