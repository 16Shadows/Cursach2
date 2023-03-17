using System;
using System.Windows.Input;
using MVVMToolbox.ViewModel;

namespace WPFToolbox.Command
{
    public class ChangeViewCommand : ICommand
    {
        #pragma warning disable CS0067
        public event EventHandler? CanExecuteChanged;
        #pragma warning restore CS0067

        protected ViewModelHost TargetModelHost { get; }
        protected Func<ViewModelBase> NewViewModel { get; }

        public ChangeViewCommand(ViewModelHost targetModelHost, Func<ViewModelBase> newViewModel)
        {
            TargetModelHost = targetModelHost ?? throw new ArgumentNullException(nameof(targetModelHost));
            NewViewModel = newViewModel ?? throw new ArgumentNullException(nameof(targetModelHost));
        }

        public bool CanExecute(object? parameter)
        {
            return TargetModelHost != null && NewViewModel != null;
        }

        public void Execute(object? parameter)
        {
            TargetModelHost.ActiveViewModel = NewViewModel.Invoke(); ;
        }
    }
}
