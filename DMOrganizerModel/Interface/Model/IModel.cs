using System;

using DMOrganizerModel.Interface.Reference;
using DMOrganizerModel.Interface.Document;
using DMOrganizerModel.Interface.NavigationTree;

namespace DMOrganizerModel.Interface.Model
{
    public class NavigationTreeReceivedEventArgs : OperationResultEventArgs
    {
        /// <summary>
        /// If the request succeeds, contains the navigation tree's root nodes
        /// </summary>
        public IObservableList<INavigationTreeNodeBase>? NavigationTree { get; init; } = null;
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

    public interface IModel : ISingleton<IModel>
    {
        /// <summary>
        /// Creates a reference to a document's section or a document
        /// </summary>
        /// <param name="section">The entity to reference (a section or a document)</param>
        /// <returns>A reference to the entity</returns>
        IReference CreateReference(ISection section);

        /// <summary>
        /// Converts a string-encoded reference to a reference object
        /// </summary>
        /// <param name="reference">String-encoded reference</param>
        /// <returns>
        /// True if the request has been queued, false otherwise
        /// </returns>
        bool DecodeReference(string reference);
        /// <summary>
        /// Is called when a reference has been decoded
        /// </summary>
        event OperationResultEventHandler<IModel, ReferenceDecodedEventArgs> ReferenceDecoded;

        /// <summary>
        /// Get the root of this model's navigation tree (which is a dummy node)
        /// </summary>
        /// <returns>True if the request has been successfully queued, false otherwise</returns>
        bool GetNavigationTree();
        /// <summary>
        /// Is called when the navigation tree has been received
        /// </summary>
        event OperationResultEventHandler<IModel, NavigationTreeReceivedEventArgs> NavigationTreeReceived;

        /// <summary>
        /// Permanently deletes all data managed by this model
        /// </summary>
        /// <returns>True if the request has been successfully queued, false otherwise</returns>
        bool DeleteData();
         /// <summary>
        /// Is called when a document has been created successfully
        /// </summary>
        event OperationResultEventHandler<IModel> DataDeleted;
    }
}
