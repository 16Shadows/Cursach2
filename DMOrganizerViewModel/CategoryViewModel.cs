using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using MVVMToolbox.Services;
using System;
using MVVMToolbox.Command;

namespace DMOrganizerViewModel
{
    public enum CategoryInputBoxScenarios
    {
        CategoryName,
        DocumentName,
        BookName
    }

    public enum CategoryNotificationScenarios
    {
        CreateCategorySuccess,
        CreateDocumentSuccess,
        CreateBookSuccess,
        DuplicateItemName
    }

    public class CategoryNotificationConfiguration : NotificationConfiguration<CategoryNotificationScenarios>
    {
        public string Name { get; }

        public CategoryNotificationConfiguration(CategoryNotificationScenarios scenario, string name) : base(scenario)
        {
            Name = name;
        }
    }

    public sealed class CategoryViewModel : NamedContainerViewModel<IOrganizerItem>
    {
        private IInputBoxService<CategoryInputBoxScenarios> CategoryInputBoxService { get; }
        private INotificationService<CategoryNotificationScenarios> CategoryNotificationService { get; }
        private string? m_CreatingCategory;
        private string? m_CreatingDocument;
        private string? m_CreatingBook;

        private ICategory Category { get; }

        public DeferredCommand CreateCategory { get; }
        public DeferredCommand CreateDocument { get; }
        public DeferredCommand CreateBook { get; }

        public CategoryViewModel(IContext context, IServiceProvider serviceProvider, ICategory category) : base(context, serviceProvider, category, category)
        {
            Category = category ?? throw new ArgumentNullException(nameof(category));

            CategoryInputBoxService = (IInputBoxService<CategoryInputBoxScenarios>)serviceProvider.GetService(typeof(IInputBoxService<CategoryInputBoxScenarios>)) ?? throw new MissingServiceException("Missing InputBoxService.");
            CategoryNotificationService = (INotificationService<CategoryNotificationScenarios>)serviceProvider.GetService(typeof(INotificationService<CategoryNotificationScenarios>)) ?? throw new MissingServiceException("Missing NotificationService.");

            Category.CategoryItemCreated.Subscribe(CategoryItemCreated);

            CreateCategory = new DeferredCommand(CommandHandler_CreateCategory, () => !LockingOperation);
            CreateDocument = new DeferredCommand(CommandHandler_CreateDocument, () => !LockingOperation);
            CreateBook = new DeferredCommand(CommandHandler_CreateBook, () => !LockingOperation);
        }

        protected override DMOrganizerViewModelBase CreateViewModel(IOrganizerItem item)
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

        private void CommandHandler_CreateCategory()
        {
            var config = new InputBoxConfiguration<CategoryInputBoxScenarios, string>(CategoryInputBoxScenarios.CategoryName, (inV, _) => inV, (inV, _) => NamingRules.IsValidName(inV) );
            InputBoxResult res = default;
            Context.Invoke(() => res = CategoryInputBoxService.Show(config));

            if (res != InputBoxResult.Success)
                return;
            Context.Invoke(() => LockingOperation = true);
            m_CreatingCategory = config.UserInput;
            Category.CreateCategory(m_CreatingCategory);            
        }

        private void CommandHandler_CreateDocument()
        {
            var config = new InputBoxConfiguration<CategoryInputBoxScenarios, string>(CategoryInputBoxScenarios.DocumentName, (inV, _) => inV, (inV, _) => NamingRules.IsValidName(inV) );
            InputBoxResult res = default;
            Context.Invoke(() => res = CategoryInputBoxService.Show(config));

            if (res != InputBoxResult.Success)
                return;
            Context.Invoke(() => LockingOperation = true);
            m_CreatingDocument = config.UserInput;
            Category.CreateDocument(m_CreatingDocument);            
        }
        //book
        private void CommandHandler_CreateBook()
        {
            var config = new InputBoxConfiguration<CategoryInputBoxScenarios, string>(CategoryInputBoxScenarios.BookName, (inV, _) => inV, (inV, _) => NamingRules.IsValidName(inV));
            InputBoxResult res = default;
            Context.Invoke(() => res = CategoryInputBoxService.Show(config));

            if (res != InputBoxResult.Success)
                return;
            Context.Invoke(() => LockingOperation = true);
            m_CreatingBook = config.UserInput;
            Category.CreateBook(m_CreatingBook);
        }

        private void CategoryItemCreated(ICategory sender, CategoryItemCreatedEventArgs e)
        {
            if (!LockingOperation)
                return;
            
            CategoryNotificationConfiguration config;
            if (m_CreatingCategory == e.Name)
            {
                config = new( e.Result == CategoryItemCreatedEventArgs.ResultType.Success ? CategoryNotificationScenarios.CreateCategorySuccess : CategoryNotificationScenarios.DuplicateItemName, e.Name );
                m_CreatingCategory = null;
            }
            else if (m_CreatingDocument == e.Name)
            {
                config = new (e.Result == CategoryItemCreatedEventArgs.ResultType.Success ? CategoryNotificationScenarios.CreateDocumentSuccess : CategoryNotificationScenarios.DuplicateItemName, e.Name );
                m_CreatingDocument = null;
            }
            else if (m_CreatingBook == e.Name)
            {
                config = new(e.Result == CategoryItemCreatedEventArgs.ResultType.Success ? CategoryNotificationScenarios.CreateBookSuccess : CategoryNotificationScenarios.DuplicateItemName, e.Name);
                m_CreatingBook = null;
            }
            else
                return;

            Context.Invoke(() =>
            {
                CategoryNotificationService.Show(config);
                LockingOperation = false;
            });
        }
    }
}
