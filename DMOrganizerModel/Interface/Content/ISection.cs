using DMOrganizerModel.Interface.NavigationTree;

namespace DMOrganizerModel.Interface.Content
{
    public interface ISection : IItem
    {
        /// <summary>
        /// The text of this section
        /// </summary>
        string Content { get; }

        /// <summary>
        /// Ordered children of this section
        /// </summary>
        IObservableReadOnlyList<ISection> Children { get; }

        /// <summary>
        /// Updates this section's content
        /// </summary>
        /// <param name="text">New content of this section</param>
        /// <returns>True if the request has been successfully queued, false otherwise</returns>
        bool UpdateContent(string text);

        /// <summary>
        /// Called when renaming has been complete
        /// </summary>
        event OperationResultEventHandler<INavigationTreeNodeBase>? ContentUpdated;
    }
}
