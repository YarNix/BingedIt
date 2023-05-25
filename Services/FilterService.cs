using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

namespace BingedIt.Services
{
    public static class FilterService<T>
    {
        static Predicate<object>? _currentFilter;

        // Passively applying Filter
        public static Predicate<object>? CurrentFilter
        {
            get => _currentFilter;
            set => _currentFilter = value;
        }

        // Actively applying/removing Filter
        public static void ApplyFilter(ICollection<T> collection, Predicate<object>? filter)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(collection);
            using (collectionView.DeferRefresh())
            {
                ApplyFilter(collectionView, filter);
            }
        }
        public static void ApplyFilter(ICollectionView collectionView, Predicate<object>? filter)
        {
            collectionView.Filter += filter;

            _currentFilter = filter;
        }
    }
}
