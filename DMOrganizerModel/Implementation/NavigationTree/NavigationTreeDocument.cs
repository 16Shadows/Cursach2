using DMOrganizerModel.Implementation.Content;
using DMOrganizerModel.Implementation.Model;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.NavigationTree;

namespace DMOrganizerModel.Implementation.NavigationTree
{
    internal class NavigationTreeDocument : NavigationTreeNodeBase, INavigationTreeDocument
    {
        #region Events
        public event OperationResultEventHandler<INavigationTreeDocument, DocumentLoadedEventArgs>? DocumentLoaded;
        #endregion

        public NavigationTreeDocument(OrganizerModel organizer, INavigationTreeRootInternal parent, string title) : base(organizer, parent, title)
        {

        }

        public Document GetDocument()
        {
            throw new System.NotImplementedException();
        }

        public override bool ChangeParent(INavigationTreeRoot newParent)
        {
            throw new System.NotImplementedException();
        }

        public bool LoadDocument()
        {
            throw new System.NotImplementedException();
        }

        public override bool Rename(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}
