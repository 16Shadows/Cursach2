using CSToolbox;
using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Interface.References;
using MVVMToolbox;
using MVVMToolbox.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DMOrganizerViewModel
{
    public class ObjectContainerViewModel : ContainerItemViewModel<IObject>
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
        public LazyProperty<int> Type { get; }
        protected IObjectContainer ObjectContainer { get; }
        public DeferredCommand CreateObject { get; }

        public ObjectContainerViewModel(IContext context, IServiceProvider serviceProvider, IObjectContainer item) : base(context, serviceProvider, item, item) 
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            else ObjectContainer = item;
            //need to set properties for Width, Height, X, Y, Type and subscribe our updater-method to listen to the model
            ObjectContainer.ObjectContainerViewInfo.Subscribe(ObjectContainer_RequestContainerViewInfo);
            ObjectContainer.ItemContainerContentChanged.Subscribe(ObjectContainer_ItemCreated);

            Width = new LazyProperty<int>(_ => ObjectContainer.RequestContainerViewInfo());
            Height = new LazyProperty<int>(_ => ObjectContainer.RequestContainerViewInfo());
            CoordX = new LazyProperty<int>(_ => ObjectContainer.RequestContainerViewInfo());
            CoordY = new LazyProperty<int>(_ => ObjectContainer.RequestContainerViewInfo());
            Type = new LazyProperty<int>(_ => ObjectContainer.RequestContainerViewInfo());

            CreateObject = new DeferredCommand(CommandHandler_CreateObject, () => !LockingOperation);
        }

        public void CommandHandler_CreateObject()
        {
            Context.Invoke(() => LockingOperation = true);
            //need to give IReferenceable objects to add object
            ObjectContainer.AddObject();
        }

        private void ObjectContainer_ItemCreated(IItemContainer<IObject> sender, ItemContainerContentChangedEventArgs<IObject> e)
        {
            if (!LockingOperation)
                return;
            Context.Invoke(() =>
            {
                LockingOperation = false;
            });
        }
        public void ObjectContainer_RequestContainerViewInfo(IItemContainer<IObject> sender, ObjectContainerViewInfoEventArgs e)
        {
            Context.Invoke(() =>
            {
                Width.Value = e.Width;
                Height.Value = e.Height;
                CoordX.Value = e.CoordX;
                CoordY.Value = e.CoordY;
                Type.Value = e.Type;
            });
        }
        protected override ItemViewModel CreateViewModel(IObject item)
        {
            return new ContainerObjectViewModel(Context, ServiceProvider, item, item);
        }
    }
}
