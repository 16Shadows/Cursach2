using CSToolbox.Weak;
using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Interface.References;
using System;

namespace DMOrganizerModel.Interface.Organizer
{
    public class OrganizerItemCreatedEventArgs : EventArgs
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

        public OrganizerItemCreatedEventArgs(string name, ResultType result = ResultType.Success)
        {
            Result = result;
            Name = name;
        }
    }

    public class ReferenceDecodedEventArgs : EventArgs
    {
        public enum ResultType
        {
            Success,
            InvalidReference
        }

        public string EncodedReference { get; }
        public IReference Instance { get; }
        public ResultType Result { get; }

        public ReferenceDecodedEventArgs(ResultType result, string encodedReference, IReference reference)
        {
            EncodedReference = encodedReference ?? throw new ArgumentNullException(nameof(encodedReference));
            Result = result;
            Instance = reference;
        }
    }

    public interface IOrganizer : IItemContainer<IOrganizerItem>
    {
        /// <summary>
        /// Is invoked when a create operation is completed.
        /// </summary>
        WeakEvent<IOrganizer, OrganizerItemCreatedEventArgs> OrganizerItemCreated { get; }

        /// <summary>
        /// Is invoked when a reference has been decoded.
        /// </summary>
        WeakEvent<IOrganizer, ReferenceDecodedEventArgs> ReferenceDecoded { get; }

        /// <summary>
        /// Creates a category in the organizer
        /// </summary>
        /// <param name="name">The name of the category</param>
        /// <exception cref="ArgumentException">Can be throw if the IOrganizer already contains a category with the same name</exception>
        void CreateCategory(string name);

        /// <summary>
        /// Creates a document in the organizer
        /// </summary>
        /// <param name="name">The name of the category</param>
        /// <exception cref="ArgumentException">Can be throw if the IOrganizer already contains a document with the same name</exception>
        void CreateDocument(string name);

        /// <summary>
        /// Creates a book in the organizer
        /// </summary>
        /// <param name="name">The name of the category</param>
        /// <exception cref="ArgumentException">Can be throw if the IOrganizer already contains a book with the same name</exception>
        void CreateBook(string name);

        /// <summary>
        /// Converts a string representation of a reference to an instance
        /// </summary>
        /// <param name="">The reference to decode</param>
        /// <returns>An instance referncing the item the encoded reference references</returns>
        void DecodeReference(string reference);
    }
}
