using CSToolbox;
using DMOrganizerModel.Interface.Items;
using DMOrganizerModel.Implementation.Utility;
using MVVMToolbox;
using MVVMToolbox.Services;
using System;
using MVVMToolbox.Command;

namespace DMOrganizerViewModel
{
    public enum ItemMessageBoxScenarios
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

    public abstract class NamedContainerViewModel<ContentType> : ContainerViewModel<ContentType> where ContentType : IItem
    {
        public LazyProperty<string?> Name { get; }

        protected INamedItem NamedItem { get; }
        protected INotificationService<NamedItemNotificationScenarios> NamedItemNotificationService { get; }
        protected IInputBoxService<NamedItemInputBoxScenarios> NamedItemInputBoxService { get; }
        
        private string? m_Renaming;
        private bool m_Deleting;

        public DeferredCommand Rename { get; }
        public DeferredCommand Delete { get; }

        protected NamedContainerViewModel(IContext context, IServiceProvider serviceProvider, INamedItem item, IItemContainer<ContentType> container) : base(context, serviceProvider, container, item)
        {
            NamedItem = item ?? throw new ArgumentNullException(nameof(item));

            NamedItemInputBoxService = (IInputBoxService<NamedItemInputBoxScenarios>)serviceProvider.GetService(typeof(IInputBoxService<NamedItemInputBoxScenarios>)) ?? throw new MissingServiceException("Missing InputBoxService.");
            NamedItemNotificationService = (INotificationService<NamedItemNotificationScenarios>)serviceProvider.GetService(typeof(INotificationService<NamedItemNotificationScenarios>)) ?? throw new MissingServiceException("Missing NotificationService.");

            NamedItem.ItemNameChanged.Subscribe(NamedItem_ItemNameChanged);
            NamedItem.ItemDeleted.Subscribe(Item_Deleted);

            Name = new LazyProperty<string?>( _ => NamedItem.RequestItemNameUpdate() );

            Rename = new DeferredCommand(CommandHandler_Rename, () => !LockingOperation);
            Delete = new DeferredCommand(CommandHandler_Delete, () => !LockingOperation);
        }

        private void NamedItem_ItemNameChanged(INamedItem sender, NamedItemNameChangedEventArgs e)
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

        private void Item_Deleted(IItem sender, ItemDeletedResult result)
        {
            if (!m_Deleting)
                return;
            NamedItemNotificationConfiguration config = new (NamedItemNotificationScenarios.ItemDeleted, Name.Value);
            Context.Invoke(() => NamedItemNotificationService.Show(config));
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

        private void CommandHandler_Delete()
        {
            //Need to add a confirmation message here, need to implement message box service
            Context.Invoke(() => LockingOperation = true);
            m_Deleting = true;
            NamedItem.DeleteItem();
        }
    }
}
