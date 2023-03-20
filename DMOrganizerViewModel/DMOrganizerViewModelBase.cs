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

        protected DMOrganizerViewModelBase(IContext context, IServiceProvider serviceProvider) : base(context, serviceProvider) { }

        protected virtual void UpdateCommandsExecutability() {}
        protected bool CanExecuteLockingOperation() => !LockingOperation;
    }
}
