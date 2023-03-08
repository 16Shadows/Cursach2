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
        public event TypedEventHandler<ISection, SectionContentChangedEventArgs> SectionContentChanged;

        /// <summary>
        /// Requests a content update for this section
        /// </summary>
        public void RequestSectionContentUpdate();
    }
}
