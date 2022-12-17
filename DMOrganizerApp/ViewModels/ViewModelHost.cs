using System;

namespace DMOrganizerApp.ViewModels
{
    internal class ViewModelHost : BaseViewModel
    {
        #region Properties
        private BaseViewModel m_ActiveViewModel;

        public BaseViewModel ActiveViewModel
        {
            get => m_ActiveViewModel;
            set
            {
                m_ActiveViewModel = value ?? throw new ArgumentNullException(nameof(ActiveViewModel));
                InvokePropertyChanged(nameof(ActiveViewModel));
            }
        }
        #endregion

        #region Constructors
        public ViewModelHost(BaseViewModel startingViewModel)
        {
            m_ActiveViewModel = startingViewModel;
        }
        #endregion
    }
}
