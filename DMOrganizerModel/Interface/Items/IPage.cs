using System;

namespace DMOrganizerModel.Interface.Items
{
    /// <summary>
    /// Page - container for object_containers. Part of book.
    /// </summary>
    public interface IPage: IItem, IItemContainer<IObjectContainer>
    {
        // position in book(int), editMode - true/false

        //create container
        //request page containers (IContainerItem)
        
        bool EditMode { get; set; }

        void RemoveContainer(IObjectContainer container); // args?
        void AddContainer(IObjectContainer container);
        void ChangePagePosition(int newPosition);
        void RequestPagePosition();
        void ChangeEditMode(bool newEditMode); //event what mode now
    }
}
