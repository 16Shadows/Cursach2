using DMOrganizerModel.Interface.Items;

namespace DMOrganizerModel.Implementation.Items
{
    internal interface INamedItemBase : INamedItem
    {
        string GetName();
        void SetName(string name);
    }
}
