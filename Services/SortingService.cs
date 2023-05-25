using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using BingedIt.Common;

namespace BingedIt.Services
{
    public static class SortingService<T>
    {
        static SortingAttribute[]? _sortingInfos;
        static string[]? _propertyNames;
        static int _index = -1;

        public static string[] PropertyNames => _propertyNames ??= InternalInitPropertyNames(SortingAttributes);
        internal static SortingAttribute[] SortingAttributes => _sortingInfos ??= InternalInitSortingAttributes();

        // Passively applying Sorting
        public static int SortIndex
        {
            get => _index;
            set => _index = value;
        }

        private static string[] InternalInitPropertyNames(SortingAttribute[] attributes) => attributes.Select(x => x.PropertyName).ToArray();
        private static SortingAttribute[] InternalInitSortingAttributes() => typeof(T).GetCustomAttributes<SortingAttribute>().ToArray();

        // Actively applying/removing Sorting
        public static void ApplySort(int index, ICollection<T> collection)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(collection);
            using (collectionView.DeferRefresh())
            {
                ApplySort(index, collectionView);
            }
        }
        public static void ApplySort(int index, ICollectionView collectionView)
        {
            var sortDescription = SortingAttributes[index].GetSortDescription();
            var groupDescription = SortingAttributes[index].GetGroupDescription();

            RemoveSort(collectionView);
            collectionView.SortDescriptions.Add(sortDescription);
            collectionView.GroupDescriptions.Add(groupDescription);

            _index = index;
        }

        internal static void ApplySort(SortingAttribute sortingInfo, ICollection<T> collection)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(collection);
            using (collectionView.DeferRefresh())
            {
                ApplySort(sortingInfo, collectionView);
            }
        }
        internal static void ApplySort(SortingAttribute sortingInfo, ICollectionView collectionView)
        {
            var sortDescription = sortingInfo.GetSortDescription();
            var groupDescription = sortingInfo.GetGroupDescription();

            RemoveSort(collectionView);
            collectionView.SortDescriptions.Add(sortDescription);
            collectionView.GroupDescriptions.Add(groupDescription);

            _index = Array.IndexOf(SortingAttributes, sortingInfo);
        }

        public static void RemoveSort(ICollection<T> collection)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(collection);
            using (collectionView.DeferRefresh())
            {
                RemoveSort(collectionView);
            }
        }
        public static void RemoveSort(ICollectionView collectionView)
        {
            collectionView.SortDescriptions.Clear();
            collectionView.GroupDescriptions.Clear();

            _index = -1;
        }

        internal static SortDescription? GetAppliedSortDescription() => int.IsPositive(_index) ? SortingAttributes[_index].GetSortDescription() : null;
        internal static GroupDescription? GetAppliedGroupDescription() => int.IsPositive(_index) ? SortingAttributes[_index].GetGroupDescription() : null;
    }
}
