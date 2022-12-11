using System.Threading.Tasks;
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
        Task UpdateContent(string text);

        /// <summary>
        /// Called when renaming has been complete
        /// </summary>
        event OperationResultEventHandler<INavigationTreeNodeBase>? ContentUpdated;

        /// <summary>
        /// Creates a subsection
        /// </summary>
        /// <param name="title">The title of the new section</param>
        Task CreateSection(string title);

        /// <summary>
        /// Called when a section has been created
        /// </summary>
        event OperationResultEventHandler<INavigationTreeNodeBase>? SectionCreated;

        /// <summary>
        /// Deletes a subsection
        /// </summary>
        /// <param name="section">The section to delete</param>
        Task DeleteSection(ISection section);
    }
}
