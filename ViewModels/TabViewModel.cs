using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using BingedIt.Commands;
using BingedIt.Services;

namespace BingedIt.ViewModels
{
    public class TabViewModel : BaseViewModel
    {
        // Using static because the viewmodel will be hosted in a ContentDialog so it's only visible for one dialog at a time
        static readonly StringViewModel _stringVM = new StringViewModel();

        private string _title;
        private ObservableCollection<MediaViewModel> _mediaList;

        readonly RelayCommand _refreshListCommand;
        readonly AddCommand<MediaViewModel> _addMediaCommand;

        readonly DialogExecuteCommand _dialogDeleteCommand;
        readonly DialogExecuteCommand _dialogRenameTitleCommand;

        public ICommand DialogDeleteCommand => _dialogDeleteCommand;
        public ICommand DialogRenameCommand => _dialogRenameTitleCommand;

        public string Title
        {
            get => _title;
            set
            {
                if (value == _title) return;
                _title = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<MediaViewModel> MediaList
        {
            get => _mediaList;
            private set
            {
                if (_mediaList == value) return;
                _mediaList = value;
                _addMediaCommand.Collection = value;
                NotifyPropertyChanged();
                if (_mediaList.Count == 0)
                    _addMediaCommand.Execute(null);
                else
                    CollectionViewSource.GetDefaultView(value).MoveCurrentTo(null);
            }
        }

        public ICommand AddMediaCommand => _addMediaCommand;
        public ICommand RefreshListCommand => _refreshListCommand;

        public TabViewModel()
        {
            _title = "New Tab";
            _mediaList = new();

            var deleteDialog = new ConfirmDialogService(title: "Delete tab", confirmText: "Yes", declineText: "No", getContent: GetDeleteDialogContent);
            var deleteSelfCommand = new DeleteSelfCommand<TabViewModel>(this);
            _dialogDeleteCommand = new(deleteSelfCommand, deleteDialog);

            var renameDialog = new ConfirmDialogService(title: "Rename tab", confirmText: "Done", declineText: "Cancel", getContent: GetRenameDialogContent);
            var renameTitleCommand = new RelayCommand(SetNewTitle);
            _dialogRenameTitleCommand = new(renameTitleCommand, renameDialog);

            _addMediaCommand = new(_mediaList);
            _refreshListCommand = new(RefreshCollection);

            // DON'T REMOVE THIS "YET"
            // List can not be empty https://github.com/dotnet/wpf/issues/7828
            _addMediaCommand.Execute(null);
        }

        private object GetDeleteDialogContent() => $"Do you want to permanently delete the tab \"{_title}\"?\nAll data contain in the tab will be deleted as well.";
        private object GetRenameDialogContent()
        {
            // Update the value to match the title
            _stringVM.Value = _title;
            return _stringVM;
        }
        private void SetNewTitle(object? o)
        {
            string newName = _stringVM.Value;
            if (string.IsNullOrEmpty(newName)) return;
            Title = newName;
        }
        private void RefreshCollection(object? _)
        {
            var collectionView = CollectionViewSource.GetDefaultView(_mediaList);
            using (IDisposable disposable = collectionView.DeferRefresh())
            {
                // When creating a new tab we don't want to apply sort and filter immediately
                // So SortingService and FilterService have passive value so they can be check against
                if (collectionView.Filter != FilterService<MediaViewModel>.CurrentFilter)
                    collectionView.Filter = FilterService<MediaViewModel>.CurrentFilter;
                if (SortingService<MediaViewModel>.GetAppliedSortDescription() is SortDescription sd)
                {
                    if (!collectionView.SortDescriptions.Contains(sd))
                        SortingService<MediaViewModel>.ApplySort(SortingService<MediaViewModel>.SortIndex, collectionView);
                }
                else
                if (collectionView.SortDescriptions.Count > 0)
                    SortingService<MediaViewModel>.RemoveSort(collectionView);
            }
            collectionView.Refresh();
        }
    }
}
