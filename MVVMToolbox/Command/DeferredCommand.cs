using System;
using System.Windows.Input;
using CSToolbox.Weak;

namespace MVVMToolbox.Command
{
    /// <summary>
    /// Implementation of ICommand which defers Execute() and CanExecute() methods to provided methods.
    /// Uses WeakDelegate regardless of construction method.
    /// </summary>
    public sealed class DeferredCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly WeakAction m_Execute;
        private readonly WeakDelegate<bool>? m_CanExecute;

        /// <summary>
        /// Passes the task to Weak version
        /// </summary>
        public DeferredCommand(WeakAction.CallType  execute, WeakDelegate<bool>.CallType? canExecute = null) : this( new WeakAction(execute), canExecute != null ? new WeakDelegate<bool>(canExecute) : null) { }

        public DeferredCommand(WeakAction execute, WeakDelegate<bool>? canExecute = null)
        {
            m_Execute = execute ?? throw new ArgumentNullException(nameof(execute));
            m_CanExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return m_CanExecute?.Invoke() != false;
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

        private readonly WeakAction<T?> m_Execute;
        private readonly WeakDelegate<bool, T?>? m_CanExecute;

        public DeferredCommand(WeakAction<T?>.CallType execute, WeakDelegate<bool, T?>.CallType? canExecute = null) : this(new WeakAction<T?>(execute), canExecute != null ? new WeakDelegate<bool, T?>(canExecute) : null) {}

        public DeferredCommand(WeakAction<T?> execute, WeakDelegate<bool, T?>? canExecute = null)
        {
            m_Execute = execute ?? throw new ArgumentNullException(nameof(execute));
            m_CanExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return m_CanExecute?.Invoke((T?)parameter) != false;
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
