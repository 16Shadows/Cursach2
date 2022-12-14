using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerApp.ViewModels
{
    internal class ViewModelHost : INotifyPropertyChanged
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

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void InvokePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Constructors
        public ViewModelHost(BaseViewModel startingViewModel)
        {
            m_ActiveViewModel = startingViewModel ?? throw new ArgumentNullException(nameof(startingViewModel));
        }
        #endregion
    }
}
