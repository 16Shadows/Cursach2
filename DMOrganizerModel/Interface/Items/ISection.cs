using System;

namespace DMOrganizerModel.Interface.Items
{
    /// <summary>
    /// Event args for SectionContentChanged of ISection.
    /// </summary>
    public class SectionContentChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new content of the section.
        /// </summary>
        public string Content { get; }

        public SectionContentChangedEventArgs(string name)
        {
            Content = name ?? throw new ArgumentNullException(nameof(name));
        }
    }

    /// <summary>
    /// A document representing a document section entity
    /// </summary>
    public interface ISection : IContainerItem<ISection>
    {
        /// <summary>
        /// Is invoked when the content of this section changes.
        /// </summary>
        event TypedEventHandler<ISection, SectionContentChangedEventArgs> SectionContentChanged;

        /// <summary>
        /// Requests a content update for this section
        /// </summary>
        /// <exception cref="InvalidOperationException">Can be thrown if the ISection has already been deleted</exception>
        void RequestSectionContentUpdate();

        /// <summary>
        /// Updates this section's content
        /// </summary>
        /// <param name="content">The content to set</param>
        /// <exception cref="InvalidOperationException">Can be thrown if the ISection has already been deleted</exception>
        void UpdateContent(string content);

        /// <summary>
        /// Creates a section in the section
        /// </summary>
        /// <param name="name">The name of the section</param>
        /// <exception cref="InvalidOperationException">Can be thrown if the ISection has already been deleted</exception>
        /// <exception cref="ArgumentException">Can be throw if the ISection already contains a section with the same name</exception>
        void CreateSection(string name);
    }
}
