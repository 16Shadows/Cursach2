﻿using CSToolbox;
using CSToolbox.Weak;
using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using System;
using System.Threading;

namespace DMOrganizerViewModel
{
    public abstract class NamedContainerViewModel<ContentType> : ContainerViewModel<ContentType> where ContentType : IItem
    {
        public LazyProperty<string?> Name { get; }

        protected INamedItem Item { get; }

        protected NamedContainerViewModel(IContext context, IServiceProvider serviceProvider, INamedItem item, IItemContainer<ContentType> container) : base(context, serviceProvider, container)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));

            Name = new LazyProperty<string?>(p =>
            {
                WeakAction<INamedItem, NamedItemNameChangedEventArgs>.CallType handler = (_, e) => Context.Invoke(() => p.Value = e.Name);
                Item.ItemNameChanged.Subscribe( handler );
                Item.RequestItemNameUpdate();
            });

            Item.ItemNameChanged.Subscribe(NamedItem_ItemNameChanged);
        }

        private void NamedItem_ItemNameChanged(INamedItem sender, NamedItemNameChangedEventArgs e)
        {
            if (!e.HasChanged)
                return;
            Context.Invoke(() => Name.Value = e.Name);
        }
    }
}
