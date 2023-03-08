using System;
using System.Collections.Generic;

namespace DMOrganizerModel.Interface.Items
{
    /// <summary>
    /// Event args for DocumentCurrentTags of IDocument
    /// </summary>
    public class DocumentCurrentTagsEventArgs : EventArgs
    {
        /// <summary>
        /// A list of current tags
        /// </summary>
        public List<string> Tags { get; }

        public DocumentCurrentTagsEventArgs(List<string> tags)
        {
            Tags = tags ?? throw new ArgumentNullException(nameof(tags));
        }
    }

    /// <summary>
    /// Event args for DocumentTagsChanged of IDocument
    /// </summary>
    public class DocumentTagsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The type of change this event represents.
        /// </summary>
        public enum ChangeType
        {
            /// <summary>
            /// A tag was added to the IDocument
            /// </summary>
            TagAdded,
            /// <summary>
            /// A tag was removed from the IDocument
            /// </summary>
            TagRemoved
        }

        /// <summary>
        /// The tag that was added or removed.
        /// </summary>
        public string Tag { get; }
        /// <summary>
        /// The type of change this event represents.
        /// </summary>
        public ChangeType Type { get; }

        public DocumentTagsChangedEventArgs(string tag, ChangeType type)
        {
            Tag = tag ?? throw new ArgumentNullException(nameof(tag));
            Type = type;
        }
    }

    /// <summary>
    /// An interface representing a document entity
    /// </summary>
    public interface IDocument : ISection
    {
        /// <summary>
        /// Is invoked when a request to this document's tags is complete.
        /// </summary>
        public event TypedEventHandler<IDocument, DocumentCurrentTagsEventArgs> DocumentCurrentTags;

        /// <summary>
        /// Is invoked when a tag is added to or removed from this document.
        /// </summary>
        public event TypedEventHandler<IDocument, DocumentTagsChangedEventArgs> DocumentTagsChanged;


        /// <summary>
        /// Request's document's current tags
        /// </summary>
        public void RequestDocumentCurrentTags();

        /// <summary>
        /// Adds a new tag to this document
        /// </summary>
        /// <param name="tag">The tag to add</param>
        /// <exception cref="ArgumentException">Can be throw if the document already has the tag</exception>
        public void AddDocumentTag(string tag);

        /// <summary>
        /// Removes a tag from this document
        /// </summary>
        /// <param name="tag">The tag to remove</param>
        /// <exception cref="ArgumentException">Can be throw if the document doesn't have the tag</exception>
        public void RemoveDocumentTag(string tag);
    }
}
