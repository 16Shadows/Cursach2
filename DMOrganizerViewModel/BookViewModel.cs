using System;
using DMOrganizerModel.Interface.Items;
using MVVMToolbox.Services;
using MVVMToolbox;
using MVVMToolbox.Command;
using DMOrganizerModel.Implementation.Utility;
using System.Windows.Controls;

namespace DMOrganizerViewModel
{
    public enum BookNotificationScenarios
    {
        CreatePageSuccess,
        Failure
    }
    public class BookNotificationConfiguration: NotificationConfiguration<BookNotificationScenarios>
    {
        public int Position { get; }
        public BookNotificationConfiguration(BookNotificationScenarios scenario, int position) : base(scenario)
        {
            Position = position;
        }
    }
    public class BookViewModel : NamedContainerViewModel<IPage>
    {
        //has INotifyPropertyChanged, method void InvokePropertyChanged(string name)
        // CreateViewModel
        // readonly LazyProperty <ObservableCollection<> Items { get; }
        // IItemContainer<IPage> Container { get; }
        // event on container items change (add, remove) - ItemContainer_ItemContainerContentChanged
        // ItemContainerContentChanged for events on add/remove item
        // event on item name changed - NamedItem_ItemNameChanged

        private INotificationService<BookNotificationScenarios> BookNotificationService { get; }
        private string m_CreatingPage;

        private IBook Book { get; }

        //commands for TreeView, deferred will be done only after request of the result
        public DeferredCommand CreatePage { get; }                                                      //!!!!!!!

        //base constructor makes property of book Name and subscribes events on it, +Items and events
        public BookViewModel(IContext context, IServiceProvider serviceProvider, IBook item, IItemContainer<IPage> container) : base(context, serviceProvider, item, container) 
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            else Book = item;
            // init services
            //BookNotificationService = (INotificationService<BookNotificationScenarios>)serviceProvider.GetService(typeof(INotificationService<BookNotificationScenarios>)) ??throw new MissingServiceException("Missing NotificationService.");

            //CreatePage = new DeferredCommand(CommandHandler_CreatePage, () => !LockingOperation);
        }

        //private void CommandHandler_CreatePage()
        //{
        //    Context.Invoke(() => LockingOperation = true);
        //    m_CreatingPage = config.UserInput;
        //    Book.CreateCategory(m_CreatingCategory);
        //}
        //creation of child page (view model ot this page)
        protected override DMOrganizerViewModelBase CreateViewModel(IPage item)
        {
            return new BookPageViewModel(Context, ServiceProvider, item, item);
        }
    }
}
