using System;
using System.Windows.Input;

namespace Calculator.Commands
{
    public class RelayCommands : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommands(Action<object> executeMethod, Predicate<object> canExecuteMethod = null)
        {
            _execute = executeMethod ?? throw new ArgumentNullException(nameof(executeMethod));
            _canExecute = canExecuteMethod;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
