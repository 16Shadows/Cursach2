using CSToolbox;
using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using System;

namespace DMOrganizerViewModel
{
    public abstract class NamedContainerViewModel<ContentType> : ContainerViewModel<ContentType> where ContentType : IItem
    {
        public LazyProperty<string?> Name { get; }

        protected INamedItem Item { get; }

        protected NamedContainerViewModel(IContext context, IServiceProvider serviceProvider, INamedItem item, IItemContainer<ContentType> container) : base(context, serviceProvider, container)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));

            Item.ItemNameChanged.Subscribe(NamedItem_ItemNameChanged);

            Name = new LazyProperty<string?>( _ => Item.RequestItemNameUpdate() );
        }

        private void NamedItem_ItemNameChanged(INamedItem sender, NamedItemNameChangedEventArgs e)
        {
            Context.Invoke(() => Name.Value = e.Name);
        }
    }
}
