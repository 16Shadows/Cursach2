using CSToolbox.Weak;
using System;

namespace DMOrganizerModel.Interface.Items
{
    public class ObjectContainerUpdateSizeEventArgs : EventArgs
    {
        public enum ResultType
        {
            /// <summary>
            /// Succesful update, size is correct
            /// </summary>
            Success,

            /// <summary>
            /// Updation failure, incorrect size
            /// </summary>
            IncorrectCoordinates
        }

        public ResultType Result { get; }
        public int Height { get; }
        public int Width { get; }
        public ObjectContainerUpdateSizeEventArgs(int width, int height, ResultType result)
        {
            Height = height;
            Width = width;
            Result = result;
        }
    }

    public class ObjectContainerUpdatePositionEventArgs: EventArgs
    {
        public enum ResultType
        {
            /// <summary>
            /// Updates succesfuly, coordinatates is correct
            /// </summary>
            Success,

            /// <summary>
            /// Updation failure, incorrect coordinates
            /// </summary>
            IncorrectCoordinates
        }

        public ResultType Result { get; }
        public int CoordX { get; }
        public int CoordY { get; }
        public ObjectContainerUpdatePositionEventArgs(int coordX, int coordY, ResultType result)
        {
            CoordX = coordX;
            CoordY = coordY;
            Result = result;
        }
    }

    public class ObjectContainerViewInfoEventArgs: EventArgs
    {

        int CoordX { get; }
        int CoordY { get; }
        int Height { get; }
        int Width { get; }

        public ObjectContainerViewInfoEventArgs(int width, int heigth, int coordX, int coordY)
        {
            CoordX = coordX;
            CoordY = coordY;
            Width = width;
            Height = heigth;
        }
    }
    /// <summary>
    /// Contains objects with links to information-objects (document, section...).
    /// </summary>
    public interface IObjectContainer : IItem, IItemContainer<IObject>
    {
        //has minimum (will set) size and maximum (page size)
        //Type(int), Height(int), Width(int), CoordX(int), CoordY(int)

        //create child-object (with "+" button maybe)
        //update coordinates X, Y
        //update height, width

        /// <summary>
        /// Is invoked when request for container's position is complete.
        /// </summary>
        WeakEvent<IObjectContainer, ObjectContainerUpdatePositionEventArgs> ObjectContainerUpdatedPosition { get; }
        
        /// <summary>
        /// 
        /// </summary>
        WeakEvent<IObjectContainer, ObjectContainerUpdateSizeEventArgs> ObjectContainerUpdatedSize { get; }

        /// <summary>
        /// Is invoked when request for gain container's size and position is complete.
        /// </summary>
        WeakEvent<IObjectContainer, ObjectContainerViewInfoEventArgs> ObjectContainerViewInfo { get; }

        /// <summary>
        /// Request for height, width, X and Y coordinates of container. 
        /// </summary>
        void RequestContainerViewInfo(); //type, height, width, coordX,Y

        /// <summary>
        /// Update coordinates for container on page, from left top corner.
        /// </summary>
        /// <param name="newX">X coordinate</param>
        /// <param name="newY">Y coordinate</param>
        void UpdateCoordinates(int newX, int newY);

        /// <summary>
        /// Update size of container.
        /// </summary>
        /// <param name="newWidth">Width</param>
        /// <param name="newHeight">Height</param>
        void UpdateSize(int newWidth, int newHeight); //need to remember about min and max size

        /// <summary>
        /// Set new object instead of old.
        /// </summary>
        /// <param name="oldObjectID">Old object ID</param>
        /// <param name="newObjectID">New object ID</param>
        void UpdateContent(int oldObjectID, int newObjectID);

        /// <summary>
        /// Creates new object in container.
        /// </summary>
        void AddObject();
    }
}
