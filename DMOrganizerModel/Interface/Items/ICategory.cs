using System;
 
namespace DMOrganizerModel.Interface.Items
{
    public class CategoryItemCreatedEventArgs : EventArgs
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

        public CategoryItemCreatedEventArgs(string name, ResultType result = ResultType.Success)
        {
            Result = result;
            Name = name;
        }
    }

    /// <summary>
    /// An interface representing a category entity
    /// </summary>
    public interface ICategory : IOrganizerItem, IItemsContainer<IOrganizerItem>
    {
        event TypedEventHandler<ICategory, CategoryItemCreatedEventArgs> CategoryItemCreated;

        /// <summary>
        /// Creates a category in the category
        /// </summary>
        /// <param name="name">The name of the category</param>
        /// <exception cref="InvalidOperationException">Can be thrown if the ICategory has already been deleted</exception>
        void CreateCategory(string name);

        /// <summary>
        /// Creates a document in the category
        /// </summary>
        /// <param name="name">The name of the category</param>
        /// <exception cref="InvalidOperationException">Can be thrown if the ICategory has already been deleted</exception>
        void CreateDocument(string name);
    }
}
