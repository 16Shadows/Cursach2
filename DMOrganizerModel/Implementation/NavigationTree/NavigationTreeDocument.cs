using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Document;
using DMOrganizerModel.Interface.NavigationTree;
using System.Text;

namespace DMOrganizerModel.Implementation.NavigationTree
{
    internal class NavigationTreeDocument : NavigationTreeNodeBase, INavigationTreeDocument
    {
        public event OperationResultEventHandler<INavigationTreeDocument, DocumentLoadedEventArgs>? DocumentLoaded;

        public bool LoadDocument()
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
