using CSToolbox;
using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Interface.References;
using MVVMToolbox;
using MVVMToolbox.Command;
using MVVMToolbox.Services;
using MVVMToolbox.ViewModel;
using System;
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
        //to change opened object views in container view
        private ViewModelBase? m_ActivePageViewModel;
        public ViewModelBase? ActivePageViewModel
        {
            get => m_ActivePageViewModel;
            set
            {
                if (m_ActivePageViewModel == value)
                    return;
                ViewModelBase? oldvm = m_ActivePageViewModel;
                if (oldvm != null)
                    Context.BeginInvoke(() => oldvm.Unload());
                m_ActivePageViewModel = value;
                m_ActivePageViewModel?.Load();
                InvokePropertyChanged(nameof(ActivePageViewModel));
            }
        }
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
            //ObjectContainer.ItemContainerContentChanged.Subscribe(ObjectContainer_SetActiveObject);
            //ObjectContainer.ItemContainerCurrentContent.Subscribe(ObjectContainer_SetActiveObjectCurrent);

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
        public void ObjectContainer_SetActiveObject(IItemContainer<IObject> sender, ItemContainerContentChangedEventArgs<IObject> e)
        {
            if (e.Type == ItemContainerContentChangedEventArgs<IObject>.ChangeType.ItemAdded
                && e.Result == ItemContainerContentChangedEventArgs<IObject>.ResultType.Success) { ActivePageViewModel = Items.Value.Last(); }
            
            else if (e.Type == ItemContainerContentChangedEventArgs<IObject>.ChangeType.ItemRemoved 
                && Items.Value.Any()) ActivePageViewModel = Items.Value.Last();

            else ActivePageViewModel = null;
        }
        public void ObjectContainer_SetActiveObjectCurrent(IItemContainer<IObject> sender, ItemContainerCurrentContentEventArgs<IObject> e)
        {
            if (e.Content.Any()) ActivePageViewModel = Items.Value.Last();
            else ActivePageViewModel = null;
        }
        public void CommandHandler_SetObject()
        {
            Context.Invoke(() => LockingOperation = true);
            //open tree window to chose object: if success - set new object by link, is cansel - nothing and no object created 
            IReferenceSelector s = ServiceProvider.GetService(typeof(IReferenceSelector)) as IReferenceSelector;
            OrganizerViewModel org = OrganizerReference.Target as OrganizerViewModel;

            IReferenceable item = s.Select(org).Item as IReferenceable;
            //need to give IReferenceable objects to add object
            if (item != null) { ObjectContainer.AddObject(item); }
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
            return new ContainerObjectViewModel(Context, ServiceProvider, item, item, OrganizerReference.Target as OrganizerViewModel);
        }
    }
}
