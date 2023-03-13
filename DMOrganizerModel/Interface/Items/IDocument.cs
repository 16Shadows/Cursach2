using CSToolbox;
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
        public IEnumerable<string> Tags { get; }

        public DocumentCurrentTagsEventArgs(IEnumerable<string> tags)
        {
            Tags = tags ?? throw new ArgumentNullException(nameof(tags));
        }
    }

    /// <summary>
    /// Event args for DocumentTagsChanged of IDocument
    /// </summary>
    public class DocumentTagsChangedEventArgs : EventArgs
    {
        public enum ResultType
        {
            /// <summary>
            /// The operation completed successfully
            /// </summary>
            Success,
            /// <summary>
            /// There is already such tag present on the document
            /// </summary>
            DuplicateTag,
            /// <summary>
            /// There is no such tag on the document
            /// </summary>
            NoSuchTag,
            /// <summary>
            /// Invalid tag value
            /// </summary>
            InvalidTag
        }

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

        /// <summary>
        /// The result of the operation
        /// </summary>
        public ResultType Result { get; }

        /// <summary>
        /// True if the tags collection has actually changed
        /// </summary>
        public bool HasChanged => Result == ResultType.Success;

        public DocumentTagsChangedEventArgs(string tag, ChangeType type, ResultType result)
        {
            Tag = tag ?? throw new ArgumentNullException(nameof(tag));
            Type = type;
            Result = result;
        }
    }

    /// <summary>
    /// An interface representing a document entity
    /// </summary>
    public interface IDocument : ISection, IOrganizerItem
    {
        /// <summary>
        /// Is invoked when a request to this document's tags is complete.
        /// </summary>
        WeakEvent<IDocument, DocumentCurrentTagsEventArgs> DocumentCurrentTags { get; }

        /// <summary>
        /// Is invoked when a tag is added to or removed from this document.
        /// </summary>
        WeakEvent<IDocument, DocumentTagsChangedEventArgs> DocumentTagsChanged { get; }


        /// <summary>
        /// Request's document's current tags
        /// </summary>
        /// <exception cref="InvalidOperationException">Can be thrown if the IDocument has already been deleted</exception>
        void RequestDocumentCurrentTags();

        /// <summary>
        /// Adds a new tag to this document
        /// </summary>
        /// <param name="tag">The tag to add</param>
        /// <exception cref="ArgumentException">Can be throw if the document already has the tag</exception>
        /// <exception cref="InvalidOperationException">Can be thrown if the IDocument has already been deleted</exception>
        void AddDocumentTag(string tag);

        /// <summary>
        /// Removes a tag from this document
        /// </summary>
        /// <param name="tag">The tag to remove</param>
        /// <exception cref="ArgumentException">Can be throw if the document doesn't have the tag</exception>
        /// <exception cref="InvalidOperationException">Can be thrown if the IDocument has already been deleted</exception>
        void RemoveDocumentTag(string tag);
    }
}
