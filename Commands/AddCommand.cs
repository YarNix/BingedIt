using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace BingedIt.Commands
{
    public sealed class AddCommand<T> : BaseCommand
    {
        ICollection<T> _collection;
        Func<object, T>? _factory;

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

        public AddCommand(ICollection<T> collection)
        {
            _collection = collection;
            _factory = null;
        }
        public AddCommand(ICollection<T> collection, Func<T> factoryFunc)
        {
            _collection = collection;
            _factory = (object o) => factoryFunc();
        }
        public AddCommand(ICollection<T> collection, Func<object, T> factoryFunc)
        {
            _collection = collection;
            _factory = factoryFunc;
        }
        public override void Execute(object? parameter = null)
        {
            T newItem;
            if (_factory is null)
                newItem = Activator.CreateInstance<T>();
            else
                newItem = _factory(parameter!);
            _collection.Add(newItem);
            CollectionViewSource.GetDefaultView(_collection).MoveCurrentTo(newItem);
        }
    }
}
