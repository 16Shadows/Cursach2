using CSToolbox;
using CSToolbox.Extensions;
using CSToolbox.Weak;
using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Interface.Organizer;
using MVVMToolbox;
using MVVMToolbox.Command;
using MVVMToolbox.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DMOrganizerViewModel
{
    public enum OrganizerInputBoxScenarios
    {
        CategoryName,
        DocumentName,
        BookName
    }

    public enum OrganizerNotificationScenarios
    {
        CreateCategorySuccess,
        CreateDocumentSuccess,
        CreateBookSuccess,
        DuplicateItemName
    }

    public class OrganizerNotificationConfiguration : NotificationConfiguration<OrganizerNotificationScenarios>
    {
        public string Name { get; }

        public OrganizerNotificationConfiguration(OrganizerNotificationScenarios scenario, string name) : base(scenario)
        {
            Name = name;
        }
    }

    public sealed class OrganizerViewModel : DMOrganizerViewModelBase
    {
        public ReadOnlyLazyProperty<ObservableCollection<ItemViewModel>?> Items { get; }

        private IOrganizer Organizer { get; }
        private IInputBoxService<OrganizerInputBoxScenarios> OrganizerInputBoxService { get; }
        private INotificationService<OrganizerNotificationScenarios> OrganizerNotificationService { get; }
        private string? m_CreatedCategory;
        private string? m_CreatedDocument;
        private string? m_CreatedBook;


        private DMOrganizerViewModelBase? m_ActiveViewModel;
        public DMOrganizerViewModelBase? ActiveViewModel
        {
            get => m_ActiveViewModel;
            set
            {
                if (m_ActiveViewModel == value)
                    return;
                m_ActiveViewModel?.Unload();
                m_ActiveViewModel = value;
                m_ActiveViewModel?.Load();
                InvokePropertyChanged(nameof(ActiveViewModel));
            }
        }
        public ObservableCollection<ItemViewModel> OpenedItems { get; } = new();

        public DeferredCommand CreateCategory { get; }
        public DeferredCommand CreateDocument { get; }
        public DeferredCommand CreateBook { get; }
        public DeferredCommand<ItemViewModel> OpenItem { get; }
        public DeferredCommand<ItemViewModel> CloseItem { get; }

        public OrganizerViewModel(IContext context, IServiceProvider serviceProvider, IOrganizer organizer) : base(context, serviceProvider)
        {
            Organizer = organizer ?? throw new ArgumentNullException(nameof(organizer));

            OrganizerInputBoxService = (IInputBoxService<OrganizerInputBoxScenarios>)serviceProvider.GetService( typeof(IInputBoxService<OrganizerInputBoxScenarios>) ) ?? throw new MissingServiceException("Missing InputBoxService.");
            OrganizerNotificationService = (INotificationService<OrganizerNotificationScenarios>)serviceProvider.GetService( typeof(INotificationService<OrganizerNotificationScenarios>) ) ?? throw new MissingServiceException("Missing NotificationService.");
            CreateCategory = new DeferredCommand(CommandHandler_CreateCategory, CanExecuteLockingOperation);
            CreateDocument = new DeferredCommand(CommandHandler_CreateDocument, CanExecuteLockingOperation);
            CreateBook = new DeferredCommand(CommandHandler_CreateBook, CanExecuteLockingOperation);

            OpenItem = new DeferredCommand<ItemViewModel>(CommandHandler_Open, target => target?.LockingOperation != true);
            CloseItem = new DeferredCommand<ItemViewModel>(CommandHandler_Close);

            Items = new ReadOnlyLazyProperty<ObservableCollection<ItemViewModel>?>(p =>
            {
                /*
                    WeakAction will hold a weak reference to the temporary class on which the lambda will be invokeds
                    So unless we hold the actual lambda (and thus its temporary class), it may get GC-ed. Probably. Need to verify.
                */
                WeakAction<IItemContainer<IOrganizerItem>, ItemContainerCurrentContentEventArgs<IOrganizerItem>>.CallType handler = (_, e) => Context.Invoke(() =>
                {
                    //Skip other possible invocations of this event until the handler is GCd
                    if (Items.Value != null)
                        return;
                    ObservableCollection<ItemViewModel> v = new ();
                    foreach (IOrganizerItem item in e.Content)
                    {
                        ItemViewModel vm = CreateViewModel(item);
                        vm.ItemDeleted.Subscribe(CommandHandler_Close);
                        v.Insert(GetViewModelPlacementIndex(vm, v), vm);
                    }
                    p(v);
                });
                Organizer.ItemContainerCurrentContent.Subscribe( handler );
                Organizer.RequestItemContainerCurrentContent();
            });

            Organizer.ItemContainerContentChanged.Subscribe(ItemContainer_ItemContainerContentChanged);

            Organizer.OrganizerItemCreated.Subscribe(OrganizerItemCreated);
        }

        private void ItemContainer_ItemContainerContentChanged(IItemContainer<IOrganizerItem> sender, ItemContainerContentChangedEventArgs<IOrganizerItem> e)
        {
            if (!e.HasChanged || Items.Value == null)
                return;
            else if (e.Type == ItemContainerContentChangedEventArgs<IOrganizerItem>.ChangeType.ItemAdded)
            {
                ItemViewModel vm = CreateViewModel(e.Item);
                vm.ItemDeleted.Subscribe(CommandHandler_Close);
                Context.Invoke(() => Items.Value.Insert(GetViewModelPlacementIndex(vm, Items.Value), vm));
            }
            else
                Context.Invoke(() => Items.Value.Remove(vm => vm.Item.Equals(e.Item)) );
        }

        private ItemViewModel CreateViewModel(IOrganizerItem item)
        {
            if (item is ICategory category)
                return new CategoryViewModel(Context, ServiceProvider, category);
            else if (item is IDocument document)
                return new DocumentViewModel(Context, ServiceProvider, document);
            else if (item is IBook book)
                return new BookViewModel(Context, ServiceProvider, book, book);
            else
                throw new ArgumentException("Unsupported item type", nameof(item));
        }

        private int GetViewModelPlacementIndex(ItemViewModel item, IList<ItemViewModel> collection)
        {
            if (item is CategoryViewModel)
            {
                int index = collection.LastIndexOf(x => x is CategoryViewModel);
                return index + 1;
            }
            else if (item is DocumentViewModel)
            {
                int index = collection.LastIndexOf(x => x is DocumentViewModel);
                return index == -1 ? collection.LastIndexOf(x => x is CategoryViewModel) + 1 : index + 1;
            }   
            else if (item is BookViewModel)
            {
                int index = collection.LastIndexOf(x => x is BookViewModel);
                if (index == -1)
                    index = collection.LastIndexOf(x => x is DocumentViewModel);
                return index == -1 ? collection.LastIndexOf(x => x is CategoryViewModel) : index + 1;
            }
            return Items.Value.Count - 1;
        }

        protected override void UpdateCommandsExecutability()
        {
            base.UpdateCommandsExecutability();
            CreateCategory.InvokeCanExecuteChanged();
            CreateDocument.InvokeCanExecuteChanged();
            CreateBook.InvokeCanExecuteChanged();
            OpenItem.InvokeCanExecuteChanged();
            CloseItem.InvokeCanExecuteChanged();
        }
        
        private void CommandHandler_Open(ItemViewModel? target)
        {
            if (target == null)
                return;
            else if (!OpenedItems.Contains(target))
                Context.Invoke(() => OpenedItems.Add(target));
            Context.Invoke(() => ActiveViewModel = target);
        }

        private void CommandHandler_Close(ItemViewModel? target)
        {
            if (target == null)
                return;
            Context.Invoke(() => OpenedItems.Remove(target));
        }

        private void CommandHandler_CreateCategory()
        {
            var config = new InputBoxConfiguration<OrganizerInputBoxScenarios, string>(OrganizerInputBoxScenarios.CategoryName, (inV, _) => inV, (inV, _) => NamingRules.IsValidName(inV) );
            InputBoxResult res = default;
            Context.Invoke(() => res = OrganizerInputBoxService.Show(config));

            if (res != InputBoxResult.Success)
                return;
            Context.Invoke(() => LockingOperation = true);
            m_CreatedCategory = config.UserInput;
            Organizer.CreateCategory(m_CreatedCategory);
        }

        private void CommandHandler_CreateDocument()
        {
            var config = new InputBoxConfiguration<OrganizerInputBoxScenarios, string>(OrganizerInputBoxScenarios.DocumentName, (inV, _) => inV, (inV, _) => NamingRules.IsValidName(inV) );
            InputBoxResult res = default;
            Context.Invoke(() => res = OrganizerInputBoxService.Show(config));

            if (res != InputBoxResult.Success)
                return;
            Context.Invoke(() => LockingOperation = true);
            m_CreatedDocument = config.UserInput;
            Organizer.CreateDocument(m_CreatedDocument);            
        }
        // book
        private void CommandHandler_CreateBook()
        {
            var config = new InputBoxConfiguration<OrganizerInputBoxScenarios, string>(OrganizerInputBoxScenarios.BookName, (inV, _) => inV, (inV, _) => NamingRules.IsValidName(inV));
            InputBoxResult res = default;
            Context.Invoke(() => res = OrganizerInputBoxService.Show(config));

            if (res != InputBoxResult.Success)
                return;
            Context.Invoke(() => LockingOperation = true);
            m_CreatedBook = config.UserInput;
            Organizer.CreateBook(m_CreatedBook);
        }

        private void OrganizerItemCreated(IOrganizer sender, OrganizerItemCreatedEventArgs e)
        {
            if (!LockingOperation)
                return;
            
            OrganizerNotificationConfiguration config;
            if (m_CreatedCategory == e.Name)
            {
                config = new( e.Result == OrganizerItemCreatedEventArgs.ResultType.Success ? OrganizerNotificationScenarios.CreateCategorySuccess : OrganizerNotificationScenarios.DuplicateItemName, e.Name );
                m_CreatedCategory = null;
            }
            else if (m_CreatedDocument == e.Name)
            {
                config = new (e.Result == OrganizerItemCreatedEventArgs.ResultType.Success ? OrganizerNotificationScenarios.CreateDocumentSuccess : OrganizerNotificationScenarios.DuplicateItemName, e.Name );
                m_CreatedDocument = null;
            }
            else if (m_CreatedBook == e.Name)
            {
                config = new(e.Result == OrganizerItemCreatedEventArgs.ResultType.Success ? OrganizerNotificationScenarios.CreateBookSuccess : OrganizerNotificationScenarios.DuplicateItemName, e.Name);
                m_CreatedBook = null;
            }
            else
                return;

            Context.Invoke(() =>
            {
                OrganizerNotificationService.Show(config);
                LockingOperation = false;
            });
        }
    }
}
