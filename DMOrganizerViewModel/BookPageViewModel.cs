using CSToolbox;
using CSToolbox.Weak;
using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using MVVMToolbox.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerViewModel
{
    public class BookPageViewModel : ContainerItemViewModel<IObjectContainer>
    {
        public LazyProperty<int> Position { get; }
        protected IPage Page { get; }
        public DeferredCommand CreateContainer { get; } 

        //base constructor only creates Container for page items, we need to make page Position and property for it, subscription on events
        public BookPageViewModel(IContext context, IServiceProvider serviceProvider, IItemContainer<IObjectContainer> container, IPage page, OrganizerViewModel org) : base(context, serviceProvider, container, page, org) 
        {
            OrganizerReference = new WeakReference(org, false);
            if (page is null) throw new ArgumentNullException(nameof(page));
            else Page = page;

            // Lazy property will be called after initialization once, when we'll ask to get Position and then will be collected with GC,
            // we need to subsribe our updater-method for further Position updates 
            //Page.PageActionCompleted.Subscribe(Page_ItemCreated);
            Page.ItemContainerContentChanged.Subscribe(Page_ItemCreated);
            Page.PageActionCompleted.Subscribe(Page_PositionChanged);
            Position = new LazyProperty<int>(_ => Page.RequestPagePosition());

            CreateContainer = new DeferredCommand(CommandHandler_CreateContainer, () => !LockingOperation);
        }

        public void CommandHandler_CreateContainer()
        {
            Context.Invoke(() => LockingOperation = true);
            //only one type of container yet
            int type = 1;
            Page.AddContainer(type);
        }
        private void Page_ItemCreated(IItemContainer<IObjectContainer> sender, ItemContainerContentChangedEventArgs<IObjectContainer> e)
        {
            if (!LockingOperation)
                return;
            Context.Invoke(() =>
            {
                LockingOperation = false;
            });
        }

        // on pageActionCompleted we will be listening and updating Position property
        private void Page_PositionChanged(IItemContainer<IObjectContainer> sender, PageActionEventArgs e)
        {
            Context.Invoke(() => Position.Value = e.Position);
        }
        protected override ItemViewModel CreateViewModel(IObjectContainer item)
        {
            OrganizerViewModel org;
            return new ObjectContainerViewModel(Context, ServiceProvider, item, OrganizerReference.Target as OrganizerViewModel);
        }
    }
}
