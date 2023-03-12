using System;

namespace DMOrganizerModel.Interface.Items
{
    /// <summary>
    /// Page - container for object_containers. Part of book.
    /// </summary>
    public interface IPage: INamedItem, IItemContainer<IObjectContainer>
    {
        //Name
    }
}
