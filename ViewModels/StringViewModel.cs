namespace BingedIt.ViewModels
{
    internal class StringViewModel : BaseViewModel
    {
        private string _value = string.Empty;
        public string Value
        {
            get => _value;
            set
            {
                if (value == _value) return;
                _value = value;
                NotifyPropertyChanged();
            }
        }
        public StringViewModel() { }
        public StringViewModel(string name)
        {
            _value = name;
        }
    }
}
