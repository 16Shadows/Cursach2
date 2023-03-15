using System;

namespace DMOrganizerModel.Interface.Items
{

    /// <summary>
    /// Page - container for object_containers. Part of book.
    /// </summary>
    public interface IPage :IItem, IItemContainer<IObjectContainer>
    {
        // position in book(int)

        //create container, event-IItemContainer
        //remove container (not deleting its content, but deleting set-bond) event-IItemContainer
        //request page containers (IContainerItem)


        /// <summary>
        /// Creates empty container on page with default size and position.
        /// </summary>
        void AddContainer();

        /// <summary>
        /// Returns current page position in parent book.
        /// </summary>
        void RequestPagePosition();

        //Need BookItemChangedEventArgs             ?????
        /// <summary>
        /// Changes position of child page, affecting db info and view. Need to change all other pages' positions.
        /// </summary>
        /// /// <param name="oldPosition">What page we're moving</param>
        /// /// <param name="newPosition">To what position we're moving</param>
        void ChangePagePosition(int oldPosition, int newPosition);
    }
}
