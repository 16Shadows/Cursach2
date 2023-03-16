using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MVVMToolbox.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        private int m_HostsCount;

        protected IContext Context { get; }
        protected IServiceProvider ServiceProvider { get; }

        public ViewModelBase(IContext context, IServiceProvider serviceProvider)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            ServiceProvider = serviceProvider;
        }

        public void Load()
        {
            if (m_HostsCount == 0)
                OnLoad();
            m_HostsCount++;
        }

        public void Unload()
        {
            m_HostsCount--;
            if (m_HostsCount == 0)
                OnUnload();
        }

        /// <summary>
        /// Is called just before this view model is hosted by a ViewModelHost for the first time after it had no hosts.
        /// </summary>
        protected virtual void OnLoad() { }

        /// <summary>
        /// Is called right after this view model is no longer hosted by any ViewModelHosts
        /// </summary>
        protected virtual void OnUnload() { }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void InvokePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion        
    }
}
