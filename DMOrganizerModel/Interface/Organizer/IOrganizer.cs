using System;
using System.Threading.Tasks;

using DMOrganizerModel.Interface.Reference;
using DMOrganizerModel.Interface.Content;
using DMOrganizerModel.Interface.NavigationTree;
using DMOrganizerModel.Interface.Items;

namespace DMOrganizerModel.Interface.Model
{
    public class NavigationTreeReceivedEventArgs : OperationResultEventArgs
    {
        /// <summary>
        /// If the request succeeds, contains the navigation tree's root nodes
        /// </summary>
        public INavigationTreeRoot? NavigationTree { get; init; } = null;
    }

    public class ReferenceDecodedEventArgs : OperationResultEventArgs
    {
        public ReferenceDecodedEventArgs(string encodedReference)
        {
            EncodedReference = encodedReference ?? throw new ArgumentNullException(nameof(encodedReference));
        }

        /// <summary>
        /// Regardless of the request's result, contains the string-encoded reference which was provided for decoding
        /// </summary>
        public string EncodedReference { get; }
        /// <summary>
        /// If the request succeeds, contains the reference object
        /// </summary>
        public IReference? ReferenceInstance { get; init; } = null;
    }

    public interface IOrganizer
    {
        /// <summary>
        /// Creates a reference to a document's section or a document
        /// </summary>
        /// <param name="section">The entity to reference</param>
        /// <returns>A reference to the entity</returns>
        /// /// <exception cref="ArgumentException">If prodvided instance is not compatiable with this model</exception>
        IReference CreateReference(IItem section);

        /// <summary>
        /// Converts a string-encoded reference to a reference object
        /// </summary>
        /// <param name="reference">String-encoded reference</param>
        /// <returns>
        /// True if the request has been queued, false otherwise
        /// </returns>
        Task DecodeReference(string reference);
        /// <summary>
        /// Is called when a reference has been decoded
        /// </summary>
        event OperationResultEventHandler<IOrganizer, ReferenceDecodedEventArgs>? ReferenceDecoded;

        /// <summary>
        /// Get the root of this model's navigation tree (which is a dummy node)
        /// </summary>
        Task GetNavigationTree();
        /// <summary>
        /// Is called when the navigation tree has been received
        /// </summary>
        event OperationResultEventHandler<IOrganizer, NavigationTreeReceivedEventArgs>? NavigationTreeReceived;

        /// <summary>
        /// Permanently deletes all data managed by this model
        /// </summary>
        /// <returns>True if the request has been successfully queued, false otherwise</returns>
        Task DeleteData();
         /// <summary>
        /// Is called when a document has been created successfully
        /// </summary>
        event OperationResultEventHandler<IOrganizer>? DataDeleted;
    }
}
