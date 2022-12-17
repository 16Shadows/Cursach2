using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMOrganizerApp.ViewModels
{
    internal class BaseViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void InvokePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
