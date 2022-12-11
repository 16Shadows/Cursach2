using System.Threading.Tasks;
using DMOrganizerModel.Interface.Content;

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
        Task LoadDocument();

        event OperationResultEventHandler<INavigationTreeDocument, DocumentLoadedEventArgs>? DocumentLoaded;
    }
}
