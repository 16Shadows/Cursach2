using System;
using System.Windows.Input;

namespace MVVMToolbox.Command
{
    /// <summary>
    /// Implementation of ICommand which defers Execute() and CanExecute() methods to provided methods
    /// </summary>
    public sealed class DeferredCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly Action m_Execute;
        private readonly Func<bool> m_CanExecute;

        public DeferredCommand(Action? execute, Func<bool>? canExecute = null)
        {
            m_Execute = execute ?? (() => { });
            m_CanExecute = canExecute ?? (() => true);
        }

        public bool CanExecute(object? parameter)
        {
            return m_CanExecute.Invoke();
        }

        public void Execute(object? parameter)
        {
            m_Execute.Invoke();
        }

        public void InvokeCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private void CommandManager_RequerySuggested(object? sender, EventArgs e)
        {
            InvokeCanExecuteChanged();
        }
    }

    /// <summary>
    /// Implementation of DeferredCommand which provides methods with a single automatically converted parameter
    /// </summary>
    /// <typeparam name="T">The type of parameter</typeparam>
    public sealed class DeferredCommand<T> : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly Action<T?> m_Execute;
        private readonly Predicate<T?> m_CanExecute;

        public DeferredCommand(Action<T?>? execute, Predicate<T?>? canExecute = null)
        {
            m_Execute = execute ?? (_ => { });
            m_CanExecute = canExecute ?? (_ => true);
        }

        public bool CanExecute(object? parameter)
        {
            return m_CanExecute.Invoke(parameter != null ? (T)parameter : default);
        }

        public void Execute(object? parameter)
        {
            m_Execute.Invoke(parameter != null ? (T)parameter : default);
        }

        public void InvokeCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private void CommandManager_RequerySuggested(object? sender, EventArgs e)
        {
            InvokeCanExecuteChanged();
        }
    }
}
