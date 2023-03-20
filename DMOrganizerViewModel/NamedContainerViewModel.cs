using CSToolbox;
using DMOrganizerModel.Implementation.Utility;
using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using MVVMToolbox.Command;
using MVVMToolbox.Services;
using System;

namespace DMOrganizerViewModel
{
    public enum NamedItemMessageBoxScenarios
    {
        Delete
    }

    public enum NamedItemInputBoxScenarios
    {
        Rename
    }

    public enum NamedItemNotificationScenarios
    {
        ItemRenamed,
        DuplicateItemName,
        ItemDeleted
    }

    public class NamedItemNotificationConfiguration : NotificationConfiguration<NamedItemNotificationScenarios>
    {
        public string Name { get; }

        public NamedItemNotificationConfiguration(NamedItemNotificationScenarios scenario, string name) : base(scenario)
        {
            Name = name;
        }
    }

    public abstract class NamedContainerViewModel<ContentType> : ContainerItemViewModel<ContentType> where ContentType : IItem
    {
        public LazyProperty<string?> Name { get; }

        protected INamedItem NamedItem { get; }
        protected INotificationService<NamedItemNotificationScenarios> NamedItemNotificationService { get; }
        protected IInputBoxService<NamedItemInputBoxScenarios> NamedItemInputBoxService { get; }
        
        protected string? m_Renaming;

        public DeferredCommand Rename { get; protected init; }

        protected NamedContainerViewModel(IContext context, IServiceProvider serviceProvider, INamedItem item, IItemContainer<ContentType> container, OrganizerViewModel org) : base(context, serviceProvider, container, item, org)
        {
            OrganizerReference = new WeakReference(org, false);
            NamedItem = item ?? throw new ArgumentNullException(nameof(item));

            NamedItemInputBoxService = (IInputBoxService<NamedItemInputBoxScenarios>)serviceProvider.GetService(typeof(IInputBoxService<NamedItemInputBoxScenarios>)) ?? throw new MissingServiceException("Missing InputBoxService.");
            NamedItemNotificationService = (INotificationService<NamedItemNotificationScenarios>)serviceProvider.GetService(typeof(INotificationService<NamedItemNotificationScenarios>)) ?? throw new MissingServiceException("Missing NotificationService.");

            NamedItem.ItemNameChanged.Subscribe(NamedItem_ItemNameChanged);

            Name = new LazyProperty<string?>( _ => NamedItem.RequestItemNameUpdate() );

            Rename = new DeferredCommand(CommandHandler_Rename, CanExecuteLockingOperation);
            Delete = new DeferredCommand(CommandHandler_Delete, CanExecuteLockingOperation);
        }

        protected virtual void NamedItem_ItemNameChanged(INamedItem sender, NamedItemNameChangedEventArgs e)
        {
            Context.Invoke(() => Name.Value = e.Name);
            if (e.Name != m_Renaming)
                return;
            NamedItemNotificationConfiguration config = new (e.Result == NamedItemNameChangedEventArgs.ResultType.Success ? NamedItemNotificationScenarios.ItemRenamed : NamedItemNotificationScenarios.DuplicateItemName, e.Name);
            m_Renaming = null;
            Context.Invoke(() =>
            {
                NamedItemNotificationService.Show(config);
                LockingOperation = false;
            });
        }

        private void CommandHandler_Delete()
        {
            //Need to add a confirmation message here, need to implement message box service
            Context.Invoke(() => LockingOperation = true);
            m_Deleting = true;
            Item.DeleteItem();
        }

        private void CommandHandler_Rename()
        {
            var config = new InputBoxConfiguration<NamedItemInputBoxScenarios, string>(NamedItemInputBoxScenarios.Rename, (inV, _) => inV, (inV, _) => NamingRules.IsValidName(inV) );
            InputBoxResult res = default;
            Context.Invoke(() => res = NamedItemInputBoxService.Show(config));

            if (res != InputBoxResult.Success)
                return;
            Context.Invoke(() => LockingOperation = true);
            m_Renaming = config.UserInput;
            NamedItem.ChangeItemName(m_Renaming);
        }

        protected override void UpdateCommandsExecutability()
        {
            base.UpdateCommandsExecutability();
            Rename.InvokeCanExecuteChanged();
            Delete.InvokeCanExecuteChanged();
        }
    }
}
