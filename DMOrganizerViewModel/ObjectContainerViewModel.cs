using CSToolbox;
using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Interface.References;
using MVVMToolbox;
using MVVMToolbox.Command;
using MVVMToolbox.Services;
using MVVMToolbox.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace DMOrganizerViewModel
{    
    //service provided by ServiceProvider
    public interface IReferenceSelector
    {
        ItemViewModel Select(OrganizerViewModel organizer);
    }

    public class ObjectContainerViewModel : ContainerItemViewModel<IObject>
    {
        public LazyProperty<int> Width { get; }
        public LazyProperty<int> Height { get; }
        public LazyProperty<int> CoordX { get; }
        public LazyProperty<int> CoordY { get; }
        public LazyProperty<int> Type { get; }

        protected IObjectContainer ObjectContainer { get; }
        public DeferredCommand SetObject { get; }
        public ObjectContainerViewModel(IContext context, IServiceProvider serviceProvider, IObjectContainer item, OrganizerViewModel org) : base(context, serviceProvider, item, item, org) 
        {
            OrganizerReference = new WeakReference(org, false);
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


            ObjectContainer.RequestItemContainerCurrentContent();
            SetObject = new DeferredCommand(CommandHandler_SetObject, CanExecuteLockingOperation);

            CoordX.WeakPropertyChanged.Subscribe(CommandHandler_ContainerCoordinatesChanged);
            CoordY.WeakPropertyChanged.Subscribe(CommandHandler_ContainerCoordinatesChanged);
            Width.WeakPropertyChanged.Subscribe(CommandHandler_ContainerSizeChanged);
            Height.WeakPropertyChanged.Subscribe(CommandHandler_ContainerSizeChanged);
        }

        public void CommandHandler_ContainerCoordinatesChanged(LazyProperty<int> e)
        {
            ObjectContainer.UpdateCoordinates(CoordX.Value, CoordY.Value);
        }
        public void CommandHandler_ContainerSizeChanged(LazyProperty<int> e)
        {
            ObjectContainer.UpdateSize(Width.Value, Height.Value);
        }
        public void CommandHandler_SetObject()
        {
            Context.Invoke(() => LockingOperation = true);
            //open tree window to chose object: if success - set new object by link, is cansel - nothing and no object created 
            IReferenceSelector s = ServiceProvider.GetService(typeof(IReferenceSelector)) as IReferenceSelector;
            OrganizerViewModel org = OrganizerReference.Target as OrganizerViewModel;

            IReferenceable item = null;
            ItemViewModel selectVM = s.Select(org);
            if (selectVM != null) { item = selectVM.Item as IReferenceable; }
            //need to give IReferenceable objects to add object
            if (item != null) 
            { 
                ObjectContainer.AddObject(item as IReferenceable);
            }
            else
            {
                Context.Invoke(() =>
                {
                    LockingOperation = false;
                });
            }
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
        // SetObject method
        protected override ItemViewModel CreateViewModel(IObject item)
        {
            return new ContainerObjectViewModel(Context, ServiceProvider, item, OrganizerReference.Target as OrganizerViewModel);
        }
    }
}
