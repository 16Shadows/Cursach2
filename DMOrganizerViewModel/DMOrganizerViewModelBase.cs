using MVVMToolbox;
using MVVMToolbox.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                InvokePropertyChanged(nameof(LockingOperation));
            }
        }

        public DMOrganizerViewModelBase(IContext context, IServiceProvider serviceProvider) : base(context, serviceProvider) {}
    }
}
