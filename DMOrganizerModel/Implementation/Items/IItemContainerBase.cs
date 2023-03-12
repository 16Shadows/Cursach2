using DMOrganizerModel.Interface.Items;

namespace DMOrganizerModel.Implementation.Items
{
    internal interface IItemContainerBase
    {
        bool CanHaveItemWithName(string name);
        void OnItemRemoved(IItem item);
    }
}
