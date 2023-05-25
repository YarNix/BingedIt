using System;

namespace BingedIt.Commands
{
    public class RelayCommand : BaseCommand
    {
        readonly Action<object?> _execute;
        readonly Predicate<object?> _canExecute;

        public RelayCommand(Action<object?> execute)
        {
            _execute = execute;
            _canExecute = base.CanExecute;
        }

        public RelayCommand(Action<object?> execute, Predicate<object?> canExecuteCommand)
        {
            _execute = execute;
            _canExecute = canExecuteCommand;
        }

        public override bool CanExecute(object? parameter) => _canExecute(parameter);
        public override void Execute(object? parameter) => _execute(parameter);
    }
}
