using System;
using System.Windows.Input;

namespace BingedIt.Commands
{
    public abstract class BaseCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        protected virtual void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, new());
        virtual public bool CanExecute(object? parameter) => true;
        abstract public void Execute(object? parameter);
    }
}
