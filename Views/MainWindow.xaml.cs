using System.IO;
using System.Reflection;
using System.Windows;
using BingedIt.ViewModels;
using YarNixSoft.Serialization;

namespace BingedIt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string SAVE_FILE_NAME = "binged.dat";
        MainViewModel? _viewModel;
        BinaryFormatter _formatter;
        public MainWindow()
        {
            _formatter = InitFormatter();
            _viewModel = InitMainViewModel(_formatter);
            DataContext = _viewModel;
            InitializeComponent();
        }

        private void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e) => SaveMainViewModel(_formatter, _viewModel);

        static BinaryFormatter InitFormatter()
        {
            using FileStream? fStream = File.Exists(SAVE_FILE_NAME) ? new(SAVE_FILE_NAME, FileMode.Open) : null;
            return fStream is not null && fStream.Length > 0 ? new(fStream) : new();
        }
        static MainViewModel InitMainViewModel(BinaryFormatter formatter)
        {
            const string VERSION = "AppVersion";
            string? versionName = formatter.Deserialize<string>(VERSION);
            string? currentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
            // BinaryFormatter is not compatible if data format change
            // So we keep a save of the sersion incase we need to make new version backward compatible
            if (versionName is null || versionName != currentVersion)
                formatter.Serialize(currentVersion, VERSION);
            return formatter.Deserialize<MainViewModel>() ?? new(); ;
        }
        static void SaveMainViewModel(BinaryFormatter formatter, MainViewModel? viewModel)
        {
            using FileStream fStream = new(SAVE_FILE_NAME, FileMode.Create);
            formatter.Serialize(viewModel);
            formatter.WriteTo(fStream);
        }

    }
}
