using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerViewModel
{
    internal class ObjectContainerViewModel : ContainerViewModel<IObject>
    {
        //has INotifyPropertyChanged, method void InvokePropertyChanged(string name)
        // CreateViewModel
        // readonly LazyProperty <ObservableCollection<> Items { get; }
        // IItemContainer<IObject> Container { get; }
        // event on container items change (add, remove) - ItemContainer_ItemContainerContentChanged
        // ItemContainerContentChanged for events on add/remove item

        public ObjectContainerViewModel(IContext context, IServiceProvider serviceProvider, IItemContainer<IObject> container) : base(context, serviceProvider, container) { }

        protected override DMOrganizerViewModelBase CreateViewModel(IObject item)
        {
            return new ContainerObjectViewModel(Context, ServiceProvider);
        }
    }
}
