using DMOrganizerModel.Interface.Document;

namespace DMOrganizerModel.Interface.NavigationTree
{
    public class DocumentLoadedEventArgs : OperationResultEventArgs
    {
        /// <summary>
        /// If the request succeeds, contains the document's object
        /// </summary>
        public IDocument? DocumentInstance { get; init; } = null;
    }

    public interface INavigationTreeDocument : INavigationTreeNodeBase
    {
        /// <summary>
        /// Loads the document this node refers to
        /// </summary>
        /// <returns>True if the request has been queued successfully, false otherwise</returns>
        bool LoadDocument();

        event OperationResultEventHandler<INavigationTreeDocument, DocumentLoadedEventArgs> DocumentLoaded;
    }
}
