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

    public interface INavigationTreeCategory : INavigationTreeNodeBase
    {
        /// <summary>
        /// Contains all children of this category, notifies on update
        /// </summary>
        IObservableList<INavigationTreeNodeBase> Children { get; }

        /// <summary>
        /// Creates a document in this category
        /// </summary>
        /// <param name="title">Title of the document</param>
        /// <returns>True if the request has been successfully queued, false otherwise</returns>
        bool CreateDocument(string title);

        /// <summary>
        /// Is called when a document has been created successfully
        /// </summary>
        event OperationResultEventHandler<INavigationTreeCategory, DocumentCreatedEventArgs> DocumentCreated;

        /// <summary>
        /// Creates a subcategory in this category
        /// </summary>
        /// <param name="title">Title of the subcategory</param>
        /// <returns>True if the request has been successfully queued, false otherwise</returns>
        bool CreateCategory(string title);

        /// <summary>
        /// Is called when a document has been created successfully
        /// </summary>
        event OperationResultEventHandler<INavigationTreeCategory, CategoryCreatedEventArgs> CategoryCreated;
    }
}
