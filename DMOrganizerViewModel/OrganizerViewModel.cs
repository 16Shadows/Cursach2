using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Interface.Organizer;
using MVVMToolbox;
using MVVMToolbox.Services;
using MVVMToolbox.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using WPFToolbox.Command;

namespace DMOrganizerViewModel
{
    public enum OrganizerInputBoxScenarios
    {
        CategoryName,
        DocumentName
    }

    public enum OrganizerNotificationScenarios
    {
        CreateCategorySuccess,
        CreateCategoryFailure,
        CreateDocumentSuccess,
        CreateDocumentFailure
    }

    public class OrganizerNotificationConfiguration : NotificationConfiguration<OrganizerNotificationScenarios>
    {
        public string Name { get; }

        public OrganizerNotificationConfiguration(OrganizerNotificationScenarios scenario, string name) : base(scenario)
        {
            Name = name;
        }
    }

    public sealed class OrganizerViewModel : ContainerViewModel<IOrganizerItem>
    {
        private IOrganizer Organizer { get; }
        private IInputBoxService<OrganizerInputBoxScenarios> InputBoxService { get; }
        private INotificationService<OrganizerNotificationScenarios> NotificationService { get; }
        private string? m_CreatedCategory;
        private string? m_CreatedDocument;

        public ObservableCollection<ViewModelBase> OpenedItems { get; } = new();

        public DeferredCommand CreateCategory { get; }
        public DeferredCommand CreateDocument { get; }

        public OrganizerViewModel(IContext context, IServiceProvider serviceProvider, IOrganizer organizer) : base(context, serviceProvider, organizer)
        {
            Organizer = organizer ?? throw new ArgumentNullException(nameof(organizer));
            //Disabled until the service is implemented
            InputBoxService = (IInputBoxService<OrganizerInputBoxScenarios>)serviceProvider.GetService( typeof(IInputBoxService<OrganizerInputBoxScenarios>) ) ?? throw new MissingServiceException("Missing InputBoxService.");
            NotificationService = (INotificationService<OrganizerNotificationScenarios>)serviceProvider.GetService( typeof(INotificationService<OrganizerNotificationScenarios>) ) ?? throw new MissingServiceException("Missing InputBoxService.");
            CreateCategory = new DeferredCommand(CommandHandler_CreateCategory, () => !LockingOperation, false);

            Organizer.OrganizerItemCreated.Subscribe(OrganizerItemCreated);
        }

        protected override DMOrganizerViewModelBase CreateViewModel(IOrganizerItem item)
        {
            if (item is ICategory category)
                return new CategoryViewModel(Context, ServiceProvider, category);
            else if (item is IDocument document)
                return new DocumentViewModel(Context, ServiceProvider, document);
            else
                throw new ArgumentException("Unsupported item type", nameof(item));
        }

        protected override void UpdateCommandsExecutability()
        {
            CreateCategory.InvokeCanExecuteChanged();
        }

        private void CommandHandler_CreateCategory()
        {
            var config = new InputBoxConfiguration<OrganizerInputBoxScenarios, string>(OrganizerInputBoxScenarios.CategoryName, (inV, _) => inV, (inV, _) => NamingRules.IsValidName(inV) );
            if (InputBoxService.Show(config) == InputBoxResult.Success)
            {
                LockingOperation = true;
                m_CreatedCategory = config.UserInput;
                Organizer.CreateCategory(m_CreatedCategory);
            }
        }

        private void OrganizerItemCreated(IOrganizer sender, OrganizerItemCreatedEventArgs e)
        {
            if (!LockingOperation)
                return;
            
            OrganizerNotificationConfiguration config;
            if (m_CreatedCategory == e.Name)
                config = new( e.Result == OrganizerItemCreatedEventArgs.ResultType.Success ? OrganizerNotificationScenarios.CreateCategorySuccess : OrganizerNotificationScenarios.CreateCategoryFailure, e.Name );
            else if (m_CreatedDocument == e.Name)
                config = new (e.Result == OrganizerItemCreatedEventArgs.ResultType.Success ? OrganizerNotificationScenarios.CreateDocumentSuccess : OrganizerNotificationScenarios.CreateDocumentFailure, e.Name );
            else
                return;

            NotificationService.Show(config);
            LockingOperation = false;
        }
    }
}
