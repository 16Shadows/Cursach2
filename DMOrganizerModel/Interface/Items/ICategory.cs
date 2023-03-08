namespace DMOrganizerModel.Interface.Items
{
    /// <summary>
    /// An interface representing a category entity
    /// </summary>
    public interface ICategory : IContainerItem<IItem>
    {
        /// <summary>
        /// Creates a category in the category
        /// </summary>
        /// <param name="name">The name of the category</param>
        /// <exception cref="InvalidOperationException">Can be thrown if the ICategory has already been deleted</exception>
        /// <exception cref="ArgumentException">Can be throw if the ICategory already contains a category with the same name</exception>
        void CreateCategory(string name);

        /// <summary>
        /// Creates a document in the category
        /// </summary>
        /// <param name="name">The name of the category</param>
        /// <exception cref="InvalidOperationException">Can be thrown if the ICategory has already been deleted</exception>
        /// <exception cref="ArgumentException">Can be throw if the ICategory already contains a document with the same name</exception>
        void CreateDocument(string name);
    }
}
