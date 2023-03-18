using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using MVVMToolbox.Command;
using MVVMToolbox.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DMOrganizerViewModel
{
    public enum ItemMessageBoxScenarios
    {
        Delete
    }

    public class ItemViewModel : DMOrganizerViewModelBase
    {
        public IItem Item { get; }

        protected bool m_Deleting;

        public DeferredCommand Delete { get; protected init; }

        protected ItemViewModel(IContext context, IServiceProvider serviceProvider, IItem item) : base(context, serviceProvider)
        {
            Item = item;

            Item.ItemDeleted.Subscribe(Item_Deleted);

            Delete = new DeferredCommand(CommandHandler_Delete, () => !LockingOperation);
        }

        protected virtual void Item_Deleted(IItem sender, ItemDeletedResult result)
        {
            if (!m_Deleting)
                return;
            m_Deleting = false;
        }

        private void CommandHandler_Delete()
        {
            //Need to add a confirmation message here, need to implement message box service
            Context.Invoke(() => LockingOperation = true);
            m_Deleting = true;
            Item.DeleteItem();
        }
    }
}
