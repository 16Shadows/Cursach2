using System;

namespace DMOrganizerModel.Interface.Items
{
    /// <summary>
    /// Contains objects with links to information-objects (document, section...).
    /// </summary>
    public interface IObjectContainer : IItem, IItemContainer<IObject>
    {
        //Type(int), Size(string?), Coordinates(string?)
    }
}
