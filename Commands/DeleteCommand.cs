using System.Collections.Generic;

namespace BingedIt.Commands
{
    public sealed class DeleteCommand<T> : BaseCommand
    {
        ICollection<T> _collection;

        public ICollection<T> Collection
        {
            get => _collection;
            set
            {
                if (_collection == value) return;
                _collection = value;
                NotifyCanExecuteChanged();
            }
        }

        public DeleteCommand(ICollection<T> collection)
        {
            _collection = collection;
        }
        public override bool CanExecute(object? parameter) => parameter is T item && _collection.Contains(item);
        public override void Execute(object? parameter) => _collection.Remove((T)parameter!);
    }
}
