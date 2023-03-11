﻿namespace MVVMToolbox
{
    public class ViewModelHost : ViewModelBase
    {
        private ViewModelBase? m_ActiveViewModel;
        public ViewModelBase? ActiveViewModel
        {
            get => m_ActiveViewModel;
            set
            {
                if (m_ActiveViewModel == value)
                    return;
                ViewModelBase? oldvm = m_ActiveViewModel;
                if (oldvm != null)
                    Context.BeginInvoke(() => oldvm.Unload());
                m_ActiveViewModel = value;
                m_ActiveViewModel?.Load();
                InvokePropertyChanged(nameof(ActiveViewModel));
            }
        }

        public ViewModelHost(IContext context, ViewModelBase? startingViewModel) : base(context)
        {
            m_ActiveViewModel = startingViewModel;
        }
    }
}