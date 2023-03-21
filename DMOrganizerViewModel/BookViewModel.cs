using CSToolbox;
using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using MVVMToolbox.Command;
using MVVMToolbox.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DMOrganizerViewModel
{
    public class BookViewModel : NamedContainerViewModel<IPage>
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
        private IBook Book { get; }

        //commands for TreeView, deferred will be done only after request of the result
        public DeferredCommand CreatePage { get; }

        //base constructor makes property of book Name and subscribes events on it, +Items and events
        public BookViewModel(IContext context, IServiceProvider serviceProvider, IBook item, IItemContainer<IPage> container, OrganizerViewModel org) : base(context, serviceProvider, item, container, org) 
        {
            OrganizerReference = new WeakReference(org, false);
            if (item is null) throw new ArgumentNullException(nameof(item));
            else Book = item;

            Book.BookItemCreated.Subscribe(Book_ItemCreated);
            Items.WeakPropertyChanged.Subscribe(Book_ItemsChanged);

            CreatePage = new DeferredCommand(CommandHandler_CreatePage, CanExecuteLockingOperation);
        }

        private void Book_ItemsChanged(ReadOnlyLazyProperty<ObservableCollection<ItemViewModel>?> arg1)
        {
            if (arg1.Value.Count > 0)
            {
                ActivePageViewModel = arg1.Value.First();
            }
            else ActivePageViewModel = null;
        }
        private void CommandHandler_CreatePage()
        {
            Context.Invoke(() => LockingOperation = true);
            Book.AddPage();
        }

        private void Book_ItemCreated(IBook sender, BookItemCreatedEventArgs e)
        {
            if (!LockingOperation)
                return;
            Context.Invoke(() =>
            {
                LockingOperation = false;
            });
        }

        //creation of child page(view model ot this page)
        protected override ItemViewModel CreateViewModel(IPage item)
        {
            return new BookPageViewModel(Context, ServiceProvider, item, item, OrganizerReference.Target as OrganizerViewModel);
        }
    }
}
