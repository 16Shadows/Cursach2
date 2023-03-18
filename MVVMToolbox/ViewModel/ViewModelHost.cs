using System;

namespace MVVMToolbox.ViewModel
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
                m_ActiveViewModel?.Unload();
                m_ActiveViewModel = value;
                m_ActiveViewModel?.Load();
                InvokePropertyChanged(nameof(ActiveViewModel));
            }
        }

        public ViewModelHost(IContext context, IServiceProvider serviceProvider, ViewModelBase? startingViewModel) : base(context, serviceProvider)
        {
            m_ActiveViewModel = startingViewModel;
        }
    }
}
