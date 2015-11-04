using System;
using System.Windows.Input;

namespace CodeOwls.SeeShell.Common.ViewModels
{
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public DelegateCommand( Action<T> execute, Func<T,bool> canExecute )
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            _execute((T) parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute((T) parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            var ev = CanExecuteChanged;
            if( null != ev)
            {
                ev(this, EventArgs.Empty);
            }
        }
    }
}