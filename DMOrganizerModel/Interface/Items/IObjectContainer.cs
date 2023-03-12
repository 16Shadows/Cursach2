using System;

namespace DMOrganizerModel.Interface.Items
{
    /// <summary>
    /// Contains objects with links to information-objects (document, section...).
    /// </summary>
    public interface IObjectContainer : IItem, IItemContainer<IObject>
    {
        //has minimum (will set) size and maximum (page size)
        //Type(int), Height(int), Width(int), CoordX(int), CoordY(int)

        //create child-object (with + button maybe)
        //update coordinates X, Y
        //update height, width
    }
}
