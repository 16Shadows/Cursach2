using DMOrganizerModel.Interface.Reference;
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

        /// <summary>
        /// True if the content has actually changed and wasn't just requested
        /// </summary>
        public bool HasChanged { get; }

        public SectionContentChangedEventArgs(string content, bool requested = false)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            HasChanged = !requested;
        }
    }

    public class SectionItemCreatedEventArgs : EventArgs
    {
        public enum ResultType
        {
            /// <summary>
            /// The item was created successfully
            /// </summary>
            Success,
            /// <summary>
            /// The item's name is invalid
            /// </summary>
            InvalidName,
            /// <summary>
            /// There is already an item with such name
            /// </summary>
            DuplicateName
        }

        public ResultType Result { get; }
        public string Name { get; }

        public SectionItemCreatedEventArgs(string name, ResultType result = ResultType.Success)
        {
            Result = result;
            Name = name;
        }
    }

    /// <summary>
    /// A document representing a document section entity
    /// </summary>
    public interface ISection : INamedItem, IItemContainer<ISection>, IReferenceable
    {
        /// <summary>
        /// Is invoked when the content of this section changes.
        /// </summary>
        event TypedEventHandler<ISection, SectionContentChangedEventArgs> SectionContentChanged;

        event TypedEventHandler<ISection, SectionItemCreatedEventArgs> SectionItemCreated;

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
