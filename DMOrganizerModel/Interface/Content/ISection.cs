using System;
using System.Threading.Tasks;
using DMOrganizerModel.Interface.NavigationTree;

namespace DMOrganizerModel.Interface.Content
{
    public class SectionCreatedEventArgs : OperationResultEventArgs
    {
        public string? Title { get; init; }
        public ISection? SectionInstance { get; init; }
    }

    public class SectionDeletedEventArgs : OperationResultEventArgs
    {
        public SectionDeletedEventArgs(string title)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
        }

        public string Title { get; }
    }

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
        event OperationResultEventHandler<ISection>? ContentUpdated;

        /// <summary>
        /// Creates a subsection
        /// </summary>
        /// <param name="title">The title of the new section</param>
        Task CreateSection(string title);

        /// <summary>
        /// Called when a section has been created
        /// </summary>
        event OperationResultEventHandler<ISection, SectionCreatedEventArgs>? SectionCreated;

        /// <summary>
        /// Deletes a subsection
        /// </summary>
        /// <param name="section">The section to delete</param>
        Task DeleteSection(ISection section);

        /// <summary>
        /// Called when a section has been created
        /// </summary>
        event OperationResultEventHandler<ISection, SectionDeletedEventArgs>? SectionDeleted;
    }
}
