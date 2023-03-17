using CSToolbox;
using CSToolbox.Weak;
using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
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

        public LazyProperty<int> Position { get; }
        protected IPage Page { get; }

        //base constructor only creates Container for page items, we need to make page Position and property for it, subscription on events
        public BookPageViewModel(IContext context, IServiceProvider serviceProvider, IItemContainer<IObjectContainer> container, IPage page) : base(context, serviceProvider, container) 
        {
            if (page is null) throw new ArgumentNullException(nameof(page));
            else Page = page;

            Position = new LazyProperty<int>(p =>
            {
                WeakAction<IPage, PageActionEventArgs>.CallType handler = (_, e) => Context.Invoke(() => Position.Value = e.Position);
                Page.PageActionCompleted.Subscribe(handler);
                Page.RequestPagePosition();
            });
            Page.ItemContainerContentChanged.Subscribe(Page_PositionChanged);
        }

        private void Page_PositionChanged(IItemContainer<IObjectContainer> sender, PageActionEventArgs e)
        {
            if (e.Position.Equals(Position))
                return;
            Context.Invoke(() => Position.Value = e.Position);

        }
        protected override DMOrganizerViewModelBase CreateViewModel(IObjectContainer item)
        {
            return new ObjectContainerViewModel(Context, ServiceProvider, item);
        }


    }
}
