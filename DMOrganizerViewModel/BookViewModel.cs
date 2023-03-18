using System;
using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using MVVMToolbox.Command;
using MVVMToolbox.Services;

namespace DMOrganizerViewModel
{
    public class BookViewModel : NamedContainerViewModel<IPage>
    {
        private int m_CreatingPage = 0;
        private IBook Book { get; }

        //commands for TreeView, deferred will be done only after request of the result
        public DeferredCommand CreatePage { get; }                                                      //!!!!!!!

        //base constructor makes property of book Name and subscribes events on it, +Items and events
        public BookViewModel(IContext context, IServiceProvider serviceProvider, IBook item, IItemContainer<IPage> container) : base(context, serviceProvider, item, container) 
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            else Book = item;

            Book.BookItemCreated.Subscribe(BookItemCreated);

            CreatePage = new DeferredCommand(CommandHandler_CreatePage, () => !LockingOperation);
        }

        private void CommandHandler_CreatePage()
        {
            Context.Invoke(() => LockingOperation = true);
            //only in last position yet
            //m_CreatingPage = Book.GetLastAvailablePagePosition(Book.);
            Book.AddPage();
        }

        private void BookItemCreated(IBook sender, BookItemCreatedEventArgs e)
        {
            if (!LockingOperation)
                return;

            //if (m_CreatingPage == e.Position)
            //{
            //    m_CreatingPage = 0;
            //}
            //else
            //    return;

            Context.Invoke(() =>
            {
                LockingOperation = false;
            });
        }

        //creation of child page(view model ot this page)
        protected override DMOrganizerViewModelBase CreateViewModel(IPage item)
        {
            return new BookPageViewModel(Context, ServiceProvider, item, item);
        }
    }
}
