using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BingedIt.Commands;
using BingedIt.Services;
using ModernWpf;

namespace BingedIt.ViewModels
{
    public sealed class MainViewModel : BaseViewModel
    {
        readonly DialogExecuteCommand _dialogSettingCommand;
        readonly AddCommand<TabViewModel> _addTabCommand;

        readonly SettingViewModel _settingVM;
        int _selectedSortIndex = -1;
        int _selectedTheme = 0;
        string _filterString = string.Empty;
        bool _caseSensitive = false;

        private ObservableCollection<TabViewModel> _tabList;

        // Local setting properties
        public bool VMCaseSensitive
        {
            get => _caseSensitive;
            private set => _caseSensitive = value;
        }
        public string VMFilterString
        {
            get => _filterString;
            private set
            {
                if (_filterString == value) return;
                bool isInputSet = !string.IsNullOrWhiteSpace(_settingVM.FilterString);
                bool isLocalSet = !string.IsNullOrWhiteSpace(_filterString);
                if (isInputSet)
                {
                    // The filter condition change. Updating the filter value
                    _filterString = _settingVM.FilterString;
                    for (int i = 0; i < _tabList.Count; i++)
                        FilterService<MediaViewModel>.ApplyFilter(_tabList[i].MediaList, FilterByTitle);
                }
                else
                if (isLocalSet)
                {
                    _filterString = string.Empty;
                    //User want to clear the filter
                    for (int i = 0; i < _tabList.Count; i++)
                        FilterService<MediaViewModel>.ApplyFilter(_tabList[i].MediaList, null);
                }
            }
        }
        public int VMSortIndex
        {
            get => _selectedSortIndex;
            private set
            {
                if (_selectedSortIndex == value) return;
                _selectedSortIndex = value;
                if (int.IsNegative(value))
                    for (int i = 0; i < _tabList.Count; i++)
                        SortingService<MediaViewModel>.RemoveSort(_tabList[i].MediaList);
                else
                    for (int i = 0; i < _tabList.Count; i++)
                        SortingService<MediaViewModel>.ApplySort(_selectedSortIndex, _tabList[i].MediaList);
            }
        }
        public int VMThemeIndex
        {
            get => _selectedTheme;
            private set
            {
                if (_selectedTheme == value) return;
                _selectedTheme = value;
                ThemeManager.Current.ApplicationTheme = _selectedTheme switch
                {
                    1 => ApplicationTheme.Light,
                    2 => ApplicationTheme.Dark,
                    _ => null
                };
            }
        }
        public ObservableCollection<TabViewModel> TabList
        {
            get => _tabList;
            private set
            {
                if (_tabList == value) return;
                _tabList = value;
                _addTabCommand.Collection = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand AddTabCommand => _addTabCommand;

        public ICommand DialogSettingCommand => _dialogSettingCommand;

        public MainViewModel()
        {
            _tabList = new();
            _addTabCommand = new(_tabList);
            _settingVM = new(SortingService<MediaViewModel>.PropertyNames);
            var settingDialog = new ConfirmDialogService(title: "Setting", confirmText: "Done", declineText: "Cancel", GetSettingVM);
            var applySettingCommand = new RelayCommand(SetSettingVM);
            _dialogSettingCommand = new(applySettingCommand, settingDialog);
        }

        SettingViewModel GetSettingVM()
        {
            // Syncing forward
            _settingVM.FilterString = _filterString;
            _settingVM.SelectedSort = _selectedSortIndex;
            _settingVM.SelectedTheme = _selectedTheme;
            return _settingVM;
        }
        void SetSettingVM(object? _)
        {
            // Syncing backward
            VMCaseSensitive = _settingVM.CaseSensitive;
            VMFilterString = _settingVM.FilterString;
            VMSortIndex = _settingVM.SelectedSort;
            VMThemeIndex = _settingVM.SelectedTheme;
        }
        bool FilterByTitle(object obj) => obj is MediaViewModel vm && vm.Title.Contains(_filterString, _caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
    }
}
