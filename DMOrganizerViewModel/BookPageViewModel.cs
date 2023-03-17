using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerViewModel
{
    internal class BookPageViewModel : ContainerViewModel<IObjectContainer>
    {
        //has INotifyPropertyChanged, method void InvokePropertyChanged(string name)
        // CreateViewModel
        // readonly LazyProperty <ObservableCollection<> Items { get; }
        // IItemContainer<IPage> Container { get; }
        // event on container items change (add, remove) - ItemContainer_ItemContainerContentChanged
        // ItemContainerContentChanged for events on add/remove item
        public BookPageViewModel(IContext context, IServiceProvider serviceProvider, IItemContainer<IObjectContainer> container) : base(context, serviceProvider, container) { }

        protected override DMOrganizerViewModelBase CreateViewModel(IObjectContainer item)
        {
            return new ObjectContainerViewModel(Context, ServiceProvider, item);
        }


    }
}
