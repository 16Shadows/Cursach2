using System;
using System.Collections.Generic;
using DMOrganizerModel.Interface.Items;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMToolbox;
using CSToolbox;

namespace DMOrganizerViewModel
{
    public class BookViewModel : NamedContainerViewModel<IPage>
    {
        //has INotifyPropertyChanged, method void InvokePropertyChanged(string name)
        // CreateViewModel
        // readonly LazyProperty <ObservableCollection<> Items { get; }
        // IItemContainer<IPage> Container { get; }
        // event on container items change (add, remove) - ItemContainer_ItemContainerContentChanged
        // ItemContainerContentChanged for events on add/remove item
        // event on item name changed - NamedItem_ItemNameChanged

        //base constructor makes property of book Name and subscribes events on it, +Items and events
        public BookViewModel(IContext context, IServiceProvider serviceProvider, INamedItem item, IItemContainer<IPage> container) : base(context, serviceProvider, item, container) { }

        //creation of child page (view model ot this page)
        protected override DMOrganizerViewModelBase CreateViewModel(IPage item)
        {
            return new BookPageViewModel(Context, ServiceProvider, item);
        }
    }
}
