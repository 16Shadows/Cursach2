using CSToolbox;
using CSToolbox.Weak;
using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using System;
using System.Collections.ObjectModel;

namespace DMOrganizerViewModel
{
    public abstract class ContainerViewModel<ContentType> : ViewModelBase where ContentType : IItem
    {
        public ReadOnlyLazyProperty<ObservableCollection<ViewModelBase>?> Items { get; }

        protected IItemContainer<ContentType> Container { get; }

        protected ContainerViewModel(IContext context, IServiceProvider serviceProvider, IItemContainer<ContentType> container) : base(context, serviceProvider)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
            
            Items = new ReadOnlyLazyProperty<ObservableCollection<ViewModelBase>?>(p =>
            {
                /*
                    WeakAction will hold a weak reference to the temporary class on which the lambda will be invokeds
                    So unless we hold the actual lambda (and thus its temporary class), it may get GC-ed.
                */
                WeakAction<IItemContainer<ContentType>, ItemContainerCurrentContentEventArgs<ContentType>>.CallType handler = (_, e) => Context.Invoke(() =>
                {
                    p.Value = new ObservableCollection<ViewModelBase>();
                    foreach (ContentType item in e.Content)
                        p.Value.Add(CreateViewModel(item));
                });
                Container.ItemContainerCurrentContent.Subscribe( handler );
                Container.RequestItemContainerCurrentContent();
            });

            Container.ItemContainerContentChanged.Subscribe(ItemContainer_ItemContainerContentChanged);
        }

        private void ItemContainer_ItemContainerContentChanged(IItemContainer<ContentType> sender, ItemContainerContentChangedEventArgs<ContentType> e)
        {
            if (!e.HasChanged || Items.Value == null)
                return;
            else if (e.Type == ItemContainerContentChangedEventArgs<ContentType>.ChangeType.ItemAdded)
                Context.Invoke(() => Items.Value.Add(CreateViewModel(e.Item)));
            else
                Context.Invoke(() => Items.Value.Remove(CreateViewModel(e.Item)));
        }

        protected abstract ViewModelBase CreateViewModel(ContentType item);
    }
}
