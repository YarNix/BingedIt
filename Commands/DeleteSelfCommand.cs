using System.Collections.Generic;

namespace BingedIt.Commands
{
    public sealed class DeleteSelfCommand<T> : BaseCommand
    {
        readonly T _item;
        public DeleteSelfCommand(T item)
        {
            _item = item;
        }
        public override bool CanExecute(object? parameter) => parameter is ICollection<T> collection && collection.Contains(_item);
        public override void Execute(object? parameter) => ((ICollection<T>)parameter!).Remove(_item);
    }
}
