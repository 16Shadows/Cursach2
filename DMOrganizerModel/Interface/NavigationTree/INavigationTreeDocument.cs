using DMOrganizerModel.Interface.Document;

namespace DMOrganizerModel.Interface.NavigationTree
{
    public interface INavigationTreeDocument : INavigationTreeNodeBase
    {
        /// <summary>
        /// The document this node refers to
        /// </summary>
        IDocument Document { get; }
    }
}
