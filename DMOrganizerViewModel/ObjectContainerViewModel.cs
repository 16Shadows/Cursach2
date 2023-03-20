using CSToolbox;
using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Interface.References;
using MVVMToolbox;
using MVVMToolbox.Command;
using MVVMToolbox.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace DMOrganizerViewModel
{
    public class ObjectContainerViewModel : ContainerItemViewModel<IObject>
    {
        //to change opened pages views in book view
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
        public LazyProperty<bool> CanHaveObject { get; }
        public UserControl parentPage { get; }

        protected IObjectContainer ObjectContainer { get; }
        public DeferredCommand CreateObject { get; }
        public ObjectContainerViewModel(IContext context, IServiceProvider serviceProvider, IObjectContainer item) : base(context, serviceProvider, item, item) 
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            else ObjectContainer = item;
            //need to set properties for Width, Height, X, Y, Type and subscribe our updater-method to listen to the model
            ObjectContainer.ObjectContainerViewInfo.Subscribe(ObjectContainer_RequestContainerViewInfo);
            ObjectContainer.ItemContainerContentChanged.Subscribe(ObjectContainer_ItemCreated);
            ObjectContainer.ItemContainerCurrentContent.Subscribe(ObjectContainer_HasObjectUpdate);
            ObjectContainer.ItemContainerContentChanged.Subscribe(ObjectContainer_SetActiveObject);
            ObjectContainer.ItemContainerCurrentContent.Subscribe(ObjectContainer_SetActiveObjectCurrent);

            Width = new LazyProperty<int>(_ => ObjectContainer.RequestContainerViewInfo());
            Height = new LazyProperty<int>(_ => ObjectContainer.RequestContainerViewInfo());
            CoordX = new LazyProperty<int>(_ => ObjectContainer.RequestContainerViewInfo());
            CoordY = new LazyProperty<int>(_ => ObjectContainer.RequestContainerViewInfo());
            Type = new LazyProperty<int>(_ => ObjectContainer.RequestContainerViewInfo());
            CanHaveObject = new LazyProperty<bool>(_ => ObjectContainer.RequestItemContainerCurrentContent());
            ObjectContainer.RequestItemContainerCurrentContent();
            CreateObject = new DeferredCommand(CommandHandler_CreateObject, () => !LockingOperation);
        }
        public void ObjectContainer_SetActiveObject(IItemContainer<IObject> sender, ItemContainerContentChangedEventArgs<IObject> e)
        {
            if (e.Type == ItemContainerContentChangedEventArgs<IObject>.ChangeType.ItemAdded && e.Result == ItemContainerContentChangedEventArgs<IObject>.ResultType.Success) { ActivePageViewModel = Items.Value.Last(); }
            else if (e.Type == ItemContainerContentChangedEventArgs<IObject>.ChangeType.ItemRemoved && Items.Value.Any()) ActivePageViewModel = Items.Value.Last();
            else ActivePageViewModel = null;
        }
        public void ObjectContainer_SetActiveObjectCurrent(IItemContainer<IObject> sender, ItemContainerCurrentContentEventArgs<IObject> e)
        {
            if (e.Content.Any()) ActivePageViewModel = Items.Value.Last();
            else ActivePageViewModel = null;
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
        public void ObjectContainer_HasObjectUpdate(IItemContainer<IObject> sender, ItemContainerCurrentContentEventArgs<IObject> e)
        {
            Context.Invoke(() =>
            {
                if (e.Content.Count() > 0) CanHaveObject.Value = false;
                else CanHaveObject.Value = true;
            });
        }
        // SetObject method
        protected override ItemViewModel CreateViewModel(IObject item)
        {
            return new ContainerObjectViewModel(Context, ServiceProvider, item, item);
        }
    }
}
