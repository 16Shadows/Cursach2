using CSToolbox.Weak;
using DMOrganizerModel.Implementation.Organizers;
using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface;
using DMOrganizerModel.Interface.Items;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerModel.Implementation.Items
{
    internal abstract class NamedContainerItem<ContentType> : ContainerItem<ContentType>, INamedItem, INamedItemBase where ContentType : IItem
    {
        protected string CachedName { get; private set; }

        protected NamedContainerItem(int itemID, IItemContainerBase parent, Organizer organizer) : base(itemID, parent, organizer)
        {
            CachedName = GetName();
        }

        #region INamedItem
        public WeakEvent<INamedItem, NamedItemNameChangedEventArgs> ItemNameChanged { get; } = new();

        protected void InvokeItemNameChanged(string newName, NamedItemNameChangedEventArgs.ResultType result)
        {
            ItemNameChanged.Invoke(this, new NamedItemNameChangedEventArgs(newName, result));
        }

        public void ChangeItemName(string newName)
        {
            CheckDeleted();
            Task.Run(() =>
            {
                if (!NamingRules.IsValidName(newName))
                {
                    InvokeItemNameChanged(newName, NamedItemNameChangedEventArgs.ResultType.InvalidName);
                    return;
                }

                bool isUnique = false;
                lock (Lock)
                {
                    isUnique = Parent.CanHaveItemWithName(newName);

                    if (isUnique)
                    {
                        SetName(newName);
                        CachedName = newName;
                    }
                }
                InvokeItemNameChanged(newName, isUnique ? NamedItemNameChangedEventArgs.ResultType.Success : NamedItemNameChangedEventArgs.ResultType.DuplicateName);
            });
        }

        public void RequestItemNameUpdate()
        {
            CheckDeleted();
            Task.Run(() => InvokeItemNameChanged(GetName(), NamedItemNameChangedEventArgs.ResultType.Requested));
        }
        #endregion

        public abstract string GetName();
        public abstract void SetName(string name);
    }
}
