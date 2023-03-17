using CSToolbox;
using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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
        public LazyProperty<int> Width { get; }
        public LazyProperty<int> Height { get; }
        public LazyProperty<int> CoordX { get; }
        public LazyProperty<int> CoordY { get; }
        public LazyProperty<int> ContainerType { get; }
        protected IObjectContainer ObjectContainer { get; }

        public ObjectContainerViewModel(IContext context, IServiceProvider serviceProvider, IItemContainer<IObject> container, IObjectContainer objectContainer) : base(context, serviceProvider, container) 
        {
            if (objectContainer is null) throw new ArgumentNullException(nameof(objectContainer));
            else ObjectContainer = objectContainer;
            //need to set properties for Width, Height, X, Y, Type
            //Width = new LazyProperty<int>(_ => ObjectContainer.RequestContainerViewInfo()[0]);
        }

        protected override DMOrganizerViewModelBase CreateViewModel(IObject item)
        {
            return new ContainerObjectViewModel(Context, ServiceProvider);
        }
    }
}
