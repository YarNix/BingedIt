using System.Windows.Input;
using BingedIt.Commands;
using ModernWpf;

namespace BingedIt.ViewModels
{
    class SettingViewModel : BaseViewModel
    {
        readonly string[] _sortingPropNames;
        readonly string[] _themeNames;
        readonly ICommand _resetCommand;
        string _filterString = string.Empty;
        int _selectedSort = -1;
        int _selectedTheme = 0;
        bool _caseSensitive = false;

        public string[] SortingPropertyNames => _sortingPropNames;
        public string[] ThemeNames => _themeNames;

        public string FilterString
        {
            get => _filterString;
            set
            {
                if (value == _filterString) return;
                _filterString = value;
                NotifyPropertyChanged();
            }
        }
        public bool CaseSensitive
        {
            get => _caseSensitive;
            set
            {
                if (value == _caseSensitive) return;
                _caseSensitive = value;
                NotifyPropertyChanged();
            }
        }
        public int SelectedSort
        {
            get => _selectedSort;
            set
            {
                if (value == _selectedSort) return;
                _selectedSort = value;
                NotifyPropertyChanged();
            }
        }
        public int SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (value == _selectedTheme) return;
                _selectedTheme = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand ResetCommand => _resetCommand;
        private void ResetProperties(object? _)
        {
            FilterString = string.Empty;
            SelectedSort = -1;
            SelectedTheme = 0;
            CaseSensitive = false;
        }

        public SettingViewModel(string[] sortingPropetyNames)
        {
            _sortingPropNames = sortingPropetyNames;
            _themeNames = new string[] { "Default", nameof(ApplicationTheme.Light), nameof(ApplicationTheme.Dark) };
            _resetCommand = new RelayCommand(ResetProperties);
        }
    }
}
