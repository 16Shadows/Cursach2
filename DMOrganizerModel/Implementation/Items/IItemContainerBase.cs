using DMOrganizerModel.Interface.Items;

namespace DMOrganizerModel.Implementation.Items
{
    internal interface IItemContainerBase
    {
        bool CanHaveItemWithName(string name);
    }

    internal interface IItemContainerBaseTyped<ContentType> : IItemContainerBase where ContentType : IItem
    {
        bool CanBeParentOf(ContentType item);
    }
}
