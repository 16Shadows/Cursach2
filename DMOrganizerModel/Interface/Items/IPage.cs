using CSToolbox.Weak;
using System;

namespace DMOrganizerModel.Interface.Items
{
    public class PageActionEventArgs : EventArgs
    {
        public enum ResultType
        {
            Success,
            Failure
        }

        public enum ActionType
        {
            ChangedPosition,
            PageDeleted,
            PageAdded
        }

        public ResultType Result { get; }
        public ActionType Action { get; }
        public int Parent { get; }

        public PageActionEventArgs(int parent, ActionType action, ResultType result = ResultType.Success)
        {
            Parent = parent;
            Action = action;
            Result = result;
        }
    }

    /// <summary>
    /// Page - container for object_containers. Part of book.
    /// </summary>
    public interface IPage :IItem, IItemContainer<IObjectContainer>
    {
        // position in book(int)

        //create container, event-IItemContainer
        //remove container (not deleting its content, but deleting set-bond) event-IItemContainer
        //request page containers (IContainerItem)

        WeakEvent<IPage, PageActionEventArgs> PageActionCompleted { get; }

        /// <summary>
        /// Creates empty container on page with default size and position.
        /// </summary>
        void AddContainer(int type);

        /// <summary>
        /// Returns current page position in parent book.
        /// </summary>
        int RequestPagePosition();

        //Need BookItemChangedEventArgs             ?????
        /// <summary>
        /// Changes position of child page, affecting db info and view. Need to change all other pages' positions.
        /// </summary>
        /// /// <param name="oldPosition">What page we're moving</param>
        /// /// <param name="newPosition">To what position we're moving</param>
        void ChangePagePosition(int bookID, int oldPosition, int newPosition);
    }
}
