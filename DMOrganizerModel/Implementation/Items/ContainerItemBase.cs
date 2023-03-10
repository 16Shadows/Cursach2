using DMOrganizerModel.Implementation.Utility;

namespace DMOrganizerModel.Implementation.Items
{
    /// <summary>
    /// A shared base class for all container items
    /// </summary>
    internal abstract class ContainerItemBase : Item
    {
        public ContainerItemBase(int itemID, ContainerItemBase parent, SyncronizedSQLiteConnection connection) : base(itemID, parent, connection) {}

        public abstract bool HasItemWithName(string name);
    }
}
