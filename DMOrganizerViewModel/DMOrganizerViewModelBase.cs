using DMOrganizerModel.Interface.Items;
using MVVMToolbox;
using MVVMToolbox.ViewModel;
using System;

namespace DMOrganizerViewModel
{
    public class DMOrganizerViewModelBase : ViewModelBase
    {
        private bool m_LockingOperation;
        public bool LockingOperation
        {
            get => m_LockingOperation;
            set
            {
                if (m_LockingOperation == value)
                    return;
                m_LockingOperation = value;
                UpdateCommandsExecutability();
                InvokePropertyChanged(nameof(LockingOperation));
            }
        }

        public IItem Item { get; }

        protected DMOrganizerViewModelBase(IContext context, IServiceProvider serviceProvider, IItem item) : base(context, serviceProvider)
        {
            Item = item;
        }

        protected virtual void UpdateCommandsExecutability() {}
    }
}
