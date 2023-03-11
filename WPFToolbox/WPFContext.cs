using MVVMToolbox;
using System;
using System.Windows.Threading;

namespace ClockApp.Utility.Context
{
    /// <summary>
    /// Wpf-specific implementation of IContext using Dispatcher
    /// </summary>
    public sealed class WPFContext : IContext
    {
        private readonly Dispatcher m_Dispatcher;
        public WPFContext(Dispatcher dispatcher)
        {
            m_Dispatcher = dispatcher;
        }

        public void BeginInvoke(Action callback)
        {
            m_Dispatcher.BeginInvoke(callback, null);
        }

        public void Invoke(Action callback)
        {
            m_Dispatcher.Invoke(callback);
        }
    }
}
