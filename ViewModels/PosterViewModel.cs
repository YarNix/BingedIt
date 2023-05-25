using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BingedIt.Services;
using YarNixSoft.Attributes;

namespace BingedIt.ViewModels
{
    public class PosterViewModel : BaseViewModel
    {
        private string _link = string.Empty;
        private string _message = string.Empty;
        private ImageSource _source;

        public string Link
        {
            get => _link;
            set
            {
                if (value == _link) return;
                ErrorMessage = string.Empty;
                bool containValue = !string.IsNullOrWhiteSpace(value);
                _link = containValue ? value : string.Empty;
                NotifyPropertyChanged();
                Source = containValue ? GetSource(value) : GetDefaultPoster();
            }
        }
        [SerialIgnore]
        public ImageSource Source
        {
            get => _source;
            private set
            {
                if (value == _source) return;
                _source = value;
                NotifyPropertyChanged(nameof(Source));
                NotifyPropertyChanged(nameof(IsLoading));
            }
        }
        public bool IsLoading => !string.IsNullOrEmpty(_link) && _source is BitmapSource source && source.IsDownloading;
        [SerialIgnore]
        public string ErrorMessage
        {
            get => _message;
            private set
            {
                if (_message == value) return;
                _message = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsValidState));
            }
        }
        public bool IsValidState => string.IsNullOrEmpty(_message) && !IsLoading;

        public PosterViewModel()
        {
            _source = GetDefaultPoster();
        }

        #region CachingLogics
        static readonly string _tempPath = $"{Path.GetTempPath()}{Process.GetCurrentProcess().ProcessName}{Path.DirectorySeparatorChar}";
        static readonly ConnectivityService _connectivity = new();
        const string SAVE_NAME = "posters.json";
        // Around 2-6 weeks
        const int EXPIRE_MIN = 14;
        const int EXPIRE_MAX = 42;
        private readonly struct Entry
        {
            public ulong File { get; }
            public DateOnly Expires { get; }
            [JsonConstructor]
            public Entry(ulong file, DateOnly expires)
            {
                File = file;
                Expires = expires;
            }
        }
        readonly static List<Entry> _entries;
        readonly static FileStream _settingStream;

        static PosterViewModel()
        {
            string settingPath = _tempPath + SAVE_NAME;
            try
            {
                if (File.Exists(settingPath))
                {
                    _settingStream = new(settingPath, FileMode.Open);
                    _entries = JsonSerializer.Deserialize<List<Entry>>(_settingStream) ?? new();
                    ValidateEntries();
                    return;
                }
            }
            catch
            {
                // Data corrupted, delete and start a new temp session
                Directory.Delete(_tempPath, true);
            }
            Directory.CreateDirectory(_tempPath);
            _settingStream = new(settingPath, FileMode.CreateNew);
            _entries = new();
            SerializeData();
        }

        static ImageSource GetDefaultPoster() => (ImageSource)Application.Current.TryFindResource("DefaultImage");
        static ImageSource GetErrorPoster() => (ImageSource)Application.Current.TryFindResource("ErrorImage");

        // Error ErrorMessage
        const string INVALID_FORMAT = "Invalid value. Value must be a path or a URL to an image\n(e.g. \"C:\\Users\\User\\Pictures\\example.png\", \"https://example.com/image.jpeg\")";
        const string FILE_NOT_EXIST = "The file specified cannot be found.";
        const string ADDRESS_NOT_EXIST = "Can't access the specified address. Please provide a valid URL.";
        const string UNSUPPORTED_FORMAT = "The provided value is not recognized as an image.\n Please make sure the image file is in a supported format (e.g., JPEG, PNG)";
        const string INTERNET_UNAVAILABLE = "No internet connection available.";

        public ImageSource GetSource(string source)
        {
            _connectivity.RemoveNotify(RetryGetSource);

            bool isExistingFile = File.Exists(source);
            if (isExistingFile)
            {
                try
                {
                    // If the file is a local file we don't need to cache it
                    return CreateLocalBitmap(Path.GetFullPath(source));
                }
                catch (NotSupportedException)
                {
                    ErrorMessage = UNSUPPORTED_FORMAT;
                }
                // File.Exist check prevent these cases
                //catch (IOException)
                //{
                //    ErrorMessage = FILE_NOT_EXIST;
                //}
                //catch (UnauthorizedAccessException)
                //{
                //    ErrorMessage = "Invalid path. File path must be accessible.";
                //}
                return GetErrorPoster();
            }
            // Create uri
            Uri uri;
            try
            {
                uri = new(source, UriKind.Absolute);
            }
            catch (UriFormatException)
            {
                ErrorMessage = INVALID_FORMAT;
                return GetErrorPoster();
            }
            // We only expect link from here
            // Check incase any file passthrough
            if (uri.IsFile)
            {
                ErrorMessage = FILE_NOT_EXIST;
                return GetErrorPoster();
            }

            // Check if the image was previously cached
            ulong hash = GetULongHash(source);
            int index = _entries.FindIndex(x => x.File == hash);
            string path = _tempPath + hash.ToString();
            var currentDate = DateOnly.FromDateTime(DateTime.Today);

            if (index >= 0 && _entries[index].Expires > currentDate)
                // Entry found and not expired. Loading image from cache
                return CreateLocalBitmap(path);

            // Image need to be redownloaded and cache
            Entry newEntry = new Entry(hash, currentDate.AddDays(Random.Shared.Next(EXPIRE_MIN, EXPIRE_MAX)));
            BitmapFrame outFrame = BitmapFrame.Create(uri);
            if (outFrame.IsDownloading)
            {
                outFrame.DownloadCompleted += OnDownloadComplete;
                outFrame.DownloadFailed += OnDownloadFailed;
            }
            else
                CacheImage(path, outFrame, newEntry, index);
            return outFrame;
            void OnDownloadComplete(object? o, EventArgs e)
            {
                var source = (BitmapFrame)o!;
                CacheImage(path, source, newEntry, index);
                NotifyPropertyChanged(nameof(IsLoading));
                // Remove handler to avoid keeping ViewModel alive
                source.DownloadCompleted -= OnDownloadComplete;
                source.DownloadFailed -= OnDownloadFailed;
            }
            void OnDownloadFailed(object? sender, ExceptionEventArgs e)
            {
                switch (e.ErrorException)
                {
                    case NotSupportedException:
                        ErrorMessage = UNSUPPORTED_FORMAT;
                        break;
                    case WebException webException:
                        if (webException.Status == WebExceptionStatus.NameResolutionFailure)
                        {
                            // Can't resolve name. Maybe offline
                            ErrorMessage = INTERNET_UNAVAILABLE;
                            _connectivity.NotifyWhenOnline(RetryGetSource);
                        }
                        else
                            ErrorMessage = ADDRESS_NOT_EXIST;
                        break;
                    default:
                        throw e.ErrorException;
                }
                Source = GetErrorPoster();
                var source = (BitmapFrame)sender!;
                // Remove handler to avoid keeping ViewModel alive
                source.DownloadCompleted -= OnDownloadComplete;
                source.DownloadFailed -= OnDownloadFailed;
            }
        }

        void RetryGetSource() => Source = GetSource(_link);

        static void CacheImage(string path, BitmapFrame frame, Entry entry, int index)
        {
            // Writing the image to a file
            using (FileStream outStream = new(path, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(frame);
                encoder.Save(outStream);
                outStream.Flush();
            }

            if (index < 0)
                // Entry not found! Adding one
                _entries.Add(entry);
            else
                // Entry found! Replace the old one
                _entries[index] = entry;
            // Update setting file
            SerializeData();
        }
        static BitmapSource CreateLocalBitmap(string fullPath)
        {
            BitmapImage source = new();
            source.BeginInit();
            source.UriSource = new Uri(fullPath);
            source.CreateOptions = BitmapCreateOptions.DelayCreation;
            source.CacheOption = BitmapCacheOption.OnDemand;
            source.EndInit();
            return source;
        }
        static void SerializeData()
        {
            _settingStream.SetLength(0);
            JsonSerializer.Serialize(_settingStream, _entries);
            _settingStream.Flush();
        }
        static void ValidateEntries()
        {
            // Delete all invalid entries
            int lengthBefore = _entries.Count;
            for (int i = 0; i < _entries.Count;)
            {
                string path = _tempPath + _entries[i].File.ToString();
                if (File.Exists(path))
                    i++;
                else
                    _entries.RemoveAt(i);
            }
            if (lengthBefore != _entries.Count)
                SerializeData();
        }
        static ulong GetULongHash(string str)
        {
            byte[] chars = Encoding.ASCII.GetBytes(str);
            Span<byte> hashOut = stackalloc byte[MD5.HashSizeInBytes];
            MD5.HashData(chars, hashOut);
            return BitConverter.ToUInt64(hashOut);
        }
        #endregion

    }
}
