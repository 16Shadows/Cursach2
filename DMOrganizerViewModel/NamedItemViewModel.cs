using CSToolbox;
using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using System;

namespace DMOrganizerViewModel
{
    public class NamedItemViewModel : ViewModelBase
    {
        public LazyProperty<string?> Name;

        protected INamedItem Item { get; }

        public NamedItemViewModel(IContext context, IServiceProvider serviceProvider, INamedItem section) : base(context, serviceProvider)
        {
            Item = section;

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
