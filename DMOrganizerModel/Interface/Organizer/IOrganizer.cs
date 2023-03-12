using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Interface.Reference;
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

    public interface IOrganizer : IItemContainer<IOrganizerItem>
    {
        /// <summary>
        /// Is invoked when a create operation is completed
        /// </summary>
        event TypedEventHandler<IOrganizer, OrganizerItemCreatedEventArgs> OrganizerItemCreated;

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
        /// Converts a string representation of a reference to an instance
        /// </summary>
        /// <param name="">The reference to decode</param>
        /// <returns>An instance referncing the item the encoded reference references</returns>
        IReference DecodeReference(string reference);
    }
}
