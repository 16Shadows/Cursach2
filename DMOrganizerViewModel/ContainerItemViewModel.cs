using CSToolbox;
using CSToolbox.Extensions;
using CSToolbox.Weak;
using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace DMOrganizerViewModel
{
    public abstract class ContainerItemViewModel<ContentType> : ItemViewModel where ContentType : IItem
    {
        public ReadOnlyLazyProperty<ObservableCollection<ItemViewModel>?> Items { get; }

        protected IItemContainer<ContentType> Container { get; }

        protected ContainerItemViewModel(IContext context, IServiceProvider serviceProvider, IItemContainer<ContentType> container, IItem item, OrganizerViewModel org) : base(context, serviceProvider, item, org)
        {
            OrganizerReference = new WeakReference(org, false);
            Container = container ?? throw new ArgumentNullException(nameof(container));
            
            Items = new ReadOnlyLazyProperty<ObservableCollection<ItemViewModel>?>(p =>
            {
                ProvideItemsValue = p;
                Container.ItemContainerCurrentContent.Subscribe( ContainerItem_CurrentContent );
                Container.RequestItemContainerCurrentContent();
            });

            Container.ItemContainerContentChanged.Subscribe(ItemContainer_ItemContainerContentChanged);
        }

        private Action<ObservableCollection<ItemViewModel>> ProvideItemsValue = null;
        private void ContainerItem_CurrentContent(IItemContainer<ContentType> _, ItemContainerCurrentContentEventArgs<ContentType> e)
        {
            Container.ItemContainerCurrentContent.Unsubscribe(ContainerItem_CurrentContent);
            ObservableCollection<ItemViewModel> v = new ();
            foreach (ContentType item in e.Content)
            {
                ItemViewModel vm = CreateViewModel(item);
                vm.ItemDeleted.Subscribe(HandleItemDeleted);
                v.Insert(GetViewModelPlacementIndex(vm, v), vm);
            }
            Context.Invoke(() => ProvideItemsValue(v));
            ProvideItemsValue = null;
        }

        private void HandleItemDeleted(ItemViewModel item)
        {
            ItemDeleted.Invoke(item);
        }

        protected override void Item_Deleted(IItem sender, ItemDeletedResult result)
        {
            if (Items.CurrentState == LazyPropertyState.Initialized)
                foreach (ItemViewModel item in Items.Value)
                    ItemDeleted.Invoke(item);
            base.Item_Deleted(sender, result);
        }

        protected virtual void ItemContainer_ItemContainerContentChanged(IItemContainer<ContentType> sender, ItemContainerContentChangedEventArgs<ContentType> e)
        {
            if (!e.HasChanged || Items.Value == null)
                return;
            else if (e.Type == ItemContainerContentChangedEventArgs<ContentType>.ChangeType.ItemAdded)
            {
                ItemViewModel vm = CreateViewModel(e.Item);
                vm.ItemDeleted.Subscribe(HandleItemDeleted);
                Context.Invoke(() => Items.Value.Insert(GetViewModelPlacementIndex(vm, Items.Value), vm));
            }
            else
                Context.Invoke(() => Items.Value.Remove(vm => vm.Item.Equals(e.Item)) );
        }

        protected abstract ItemViewModel CreateViewModel(ContentType item);
        protected virtual int GetViewModelPlacementIndex(ItemViewModel item, IList<ItemViewModel> collection)
        {
            return Math.Max(collection.Count - 1, 0);
        }
    }
}
