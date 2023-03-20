using CSToolbox;
using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Interface.References;
using MVVMToolbox;
using System;

namespace DMOrganizerViewModel
{
    public class ContainerObjectViewModel : ContainerItemViewModel<IReferenceable>
    {
        //to change opened referenceable views in object view
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

        // ContainerViewModel already keeps all Object IReferenceable objects, we just need to show them in proper way
        public LazyProperty<IReferenceable> ReferenceableObjectType { get; }
        protected IObject ContainerObject { get; }
        public ContainerObjectViewModel(IContext context, IServiceProvider serviceProvider, IItemContainer<IReferenceable> container, IObject item, OrganizerViewModel org) : base(context, serviceProvider, container, item, org)
        {
            OrganizerReference = new WeakReference(org, false);
            if (item is null) throw new ArgumentNullException(nameof(item));
            else ContainerObject = item;

            ContainerObject.ItemContainerContentChanged.Subscribe(ContainerObject_SetActiveObject);
            ContainerObject.ItemContainerCurrentContent.Subscribe(ContainerObject_SetActiveObjectCurrent);
        }

        public void ContainerObject_SetActiveObject(IItemContainer<IReferenceable> sender, ItemContainerContentChangedEventArgs<IReferenceable> e)
        {
            if (e.Type == ItemContainerContentChangedEventArgs<IReferenceable>.ChangeType.ItemAdded
                && e.Result == ItemContainerContentChangedEventArgs<IReferenceable>.ResultType.Success) { ActivePageViewModel = Items.Value.Last(); }

            else if (e.Type == ItemContainerContentChangedEventArgs<IReferenceable>.ChangeType.ItemRemoved
                && Items.Value.Any()) ActivePageViewModel = Items.Value.Last();

            else ActivePageViewModel = null;
        }
        public void ContainerObject_SetActiveObjectCurrent(IItemContainer<IReferenceable> sender, ItemContainerCurrentContentEventArgs<IReferenceable> e)
        {
            if (e.Content.Any()) ActivePageViewModel = Items.Value.Last();
            else ActivePageViewModel = null;
        }
        protected override ItemViewModel CreateViewModel(IReferenceable item)
        {            
            //type check and creation of different child-objects
            if (item is IDocument)
            {
                DocumentViewModel doc = new DocumentViewModel(Context, ServiceProvider, item as IDocument, OrganizerReference.Target as OrganizerViewModel);
                ActivePageViewModel = doc;
                return doc;
            }
            else if (item is ISection)
            {
                SectionViewModel sec =  new SectionViewModel(Context, ServiceProvider, item as ISection, OrganizerReference.Target as OrganizerViewModel);
                ActivePageViewModel = sec;
                return sec;
            }
            else throw new ArgumentException("Unsupported Referencable type");

        }
    }
}
