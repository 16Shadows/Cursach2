using DMOrganizerModel.Interface.Content;
using System.Threading.Tasks;

namespace DMOrganizerModel.Interface.NavigationTree
{
    public class DocumentCreatedEventArgs : OperationResultEventArgs
    {
        /// <summary>
        /// If the request succeeds, contains the document's object
        /// </summary>
        public INavigationTreeDocument? DocumentInstance { get; init; } = null;
    }

    public class CategoryCreatedEventArgs : OperationResultEventArgs
    {
        /// <summary>
        /// If the request succeeds, contains the document's object
        /// </summary>
        public INavigationTreeCategory? CategoryInstance { get; init; } = null;
    }

    public interface INavigationTreeRoot
    {
        /// <summary>
        /// Contains all children of this category, notifies on update
        /// </summary>
        IObservableReadOnlyCollection<INavigationTreeNodeBase> Children { get; }

        /// <summary>
        /// Creates a document in this category
        /// </summary>
        /// <param name="title">Title of the document</param>
        /// <returns>True if the request has been successfully queued, false otherwise</returns>
        Task CreateDocument(string title);

        /// <summary>
        /// Is called when a document has been created successfully
        /// </summary>
        event OperationResultEventHandler<INavigationTreeRoot, DocumentCreatedEventArgs>? DocumentCreated;

        /// <summary>
        /// Creates a subcategory in this category
        /// </summary>
        /// <param name="title">Title of the subcategory</param>
        /// <returns>True if the request has been successfully queued, false otherwise</returns>
        Task CreateCategory(string title);

        /// <summary>
        /// Is called when a document has been created successfully
        /// </summary>
        event OperationResultEventHandler<INavigationTreeRoot, CategoryCreatedEventArgs>? CategoryCreated;

        /// <summary>
        /// Deletes a document
        /// </summary>
        /// <param name="document">The document to delete</param>
        /// <returns>True if the request has been successfully queued, false otherwise</returns>
        Task DeleteDocument(INavigationTreeDocument document);

        /// <summary>
        /// Is called when a document has been created successfully
        /// </summary>
        event OperationResultEventHandler<INavigationTreeRoot>? DocumentDeleted;

        /// <summary>
        /// Deletes a subcategory
        /// </summary>
        /// <param name="title">The subcategory to delete</param>
        /// <returns>True if the request has been successfully queued, false otherwise</returns>
        Task DeleteCategory(INavigationTreeCategory category);

        /// <summary>
        /// Is called when a document has been created successfully
        /// </summary>
        event OperationResultEventHandler<INavigationTreeRoot>? CategoryDeleted;
    }
}
