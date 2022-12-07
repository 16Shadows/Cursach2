using DMOrganizerModel.Interface.NavigationTree;

namespace DMOrganizerModel.Interface.Document
{
    public interface ISection
    {
        /// <summary>
        /// The title of this section
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Renames this section
        /// </summary>
        /// <param name="name">New title of this section</param>
        /// <returns>True if the request has been successfully queued, false otherwise</returns>
        bool Rename(string name);

        /// <summary>
        /// Called when renaming has been complete
        /// </summary>
        event OperationResultEventHandler<INavigationTreeNodeBase> Renamed;

        /// <summary>
        /// The text of this section
        /// </summary>
        string Content { get; }

        /// <summary>
        /// Updates this section's content
        /// </summary>
        /// <param name="text">New content of this section</param>
        /// <returns>True if the request has been successfully queued, false otherwise</returns>
        bool UpdateContent(string text);

        /// <summary>
        /// Called when renaming has been complete
        /// </summary>
        event OperationResultEventHandler<INavigationTreeNodeBase> ContentUpdated;

        /// <summary>
        /// Ordered children of this section
        /// </summary>
        IObservableList<ISection> Children { get; }
    }
}
