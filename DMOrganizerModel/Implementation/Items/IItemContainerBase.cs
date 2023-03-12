using DMOrganizerModel.Interface.Items;

namespace DMOrganizerModel.Implementation.Items
{
    internal interface IItemContainerBase
    {
        bool CanHaveItemWithName(string name);
        void OnItemRemoved(IItem item);
    }

    internal interface IItemContainerBaseTyped<ContentType> : IItemContainerBase, IItemContainer<ContentType> where ContentType : IItem
    {
        bool CanBeParentOf(ContentType item);
    }
}
