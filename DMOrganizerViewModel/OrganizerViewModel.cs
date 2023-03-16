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
    enum OrganizerInputBoxScenarios
    {
        CategoryName,
        DocumentName
    }

    public sealed class OrganizerViewModel : ContainerViewModel<IOrganizerItem>
    {
        private IOrganizer Organizer { get; }
        private IInputBoxService<OrganizerInputBoxScenarios> InputBoxService { get; }
        private string? m_CreatedCategory;

        public ObservableCollection<ViewModelBase> OpenedItems { get; } = new();

        public DeferredCommand CreateCategory { get; }

        public OrganizerViewModel(IContext context, IServiceProvider serviceProvider, IOrganizer organizer) : base(context, serviceProvider, organizer)
        {
            Organizer = organizer ?? throw new ArgumentNullException(nameof(organizer));
            //Disabled until the service is implemented
            //InputBoxService = (IInputBoxService<OrganizerInputBoxScenarios>)serviceProvider.GetService( typeof(IInputBoxService<OrganizerInputBoxScenarios>) ) ?? throw new MissingServiceException("Missing InputBoxService.");
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

        private void CommandHandler_CreateCategory()
        {
            var config = new InputBoxConfiguration<OrganizerInputBoxScenarios, string>(OrganizerInputBoxScenarios.CategoryName, (string inV, out string? outV, CultureInfo _) => NamingRules.IsValidName(outV = inV) );
            if (InputBoxService.Show(config) == InputBoxResult.Success)
            {
                LockingOperation = true;
                CreateCategory.InvokeCanExecuteChanged();
                m_CreatedCategory = config.UserInput;
                Organizer.CreateCategory(m_CreatedCategory);
            }
        }

        private void OrganizerItemCreated(IOrganizer sender, OrganizerItemCreatedEventArgs e)
        {
            if (!LockingOperation || m_CreatedCategory != e.Name)
                return;
            
        }
    }
}
