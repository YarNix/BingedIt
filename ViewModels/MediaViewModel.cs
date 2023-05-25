using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using BingedIt.Commands;
using BingedIt.Common;
using BingedIt.Converters.Specialized;
using BingedIt.Services;
using YarNixSoft.Attributes;

namespace BingedIt.ViewModels
{
    [Sorting(nameof(Title), ValueConverterType = typeof(StringFirstLetterConverter)),
     Sorting(nameof(Rating), DefaultDirection = ListSortDirection.Descending, ValueConverterType = typeof(JoinedRatingToStringConverter)),
     Sorting(nameof(Priority), DefaultDirection = ListSortDirection.Descending, ValueConverterType = typeof(BingePriorityToStringConverter))]
    public sealed class MediaViewModel : BaseViewModel
    {
        readonly static ConfirmDialogService _dialog = new("Delete Confirmation", "Yes", "No", () => "Are you sure you want to delete this?");
        readonly static RelayCommand _randomizeBrushCommand = new(Randomize, CanRandomize);
        readonly static PosterViewModel _tmpPosterVM = new();

        private static bool CanRandomize(object? parameter) => parameter is MediaViewModel;
        private static void Randomize(object? parameter)
        {
            MediaViewModel self = (MediaViewModel)parameter!;
            var brushes = BrushesProvider.ProvideValue();
            int index = Random.Shared.Next(brushes.Length);
            self.Foreground = brushes[index];
        }
        public static ICommand RandomizeBrushCommand => _randomizeBrushCommand;

        private string _title;
        private string _description;
        private SolidColorBrush _titleFGBrush;
        private PosterViewModel _posterVM;
        private ObservableCollection<SeasonViewModel> _seasons;

        readonly DialogExecuteCommand _dialodEditPosterCommand;

        readonly DialogExecuteCommand _dialogDeleteCommand;

        readonly AddCommand<SeasonViewModel> _addSeasonCommand;
        readonly DialogExecuteCommand _dialogDeleteSeasonCommand;
        readonly RelayCommand _moveUpSeasonCommand;
        readonly RelayCommand _moveDownSeasonCommand;

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
        public string Description
        {
            get => _description;
            set
            {
                if (value == _description) return;
                _description = value;
                NotifyPropertyChanged();
            }
        }
        [SerialIgnore]
        public SolidColorBrush Foreground
        {
            get => _titleFGBrush;
            set
            {
                if (value == _titleFGBrush) return;
                _titleFGBrush = value;
                NotifyPropertyChanged();
            }
        }
        public PosterViewModel Poster
        {
            get => _posterVM;
            private set
            {
                if (value == _posterVM) return;
                _posterVM = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<SeasonViewModel> Seasons
        {
            get => _seasons;
            private set
            {
                if (value == _seasons) return;
                _seasons = value;
                NotifyPropertyChanged();
                _addSeasonCommand.Collection = value;
                NotifyPropertyChanged(nameof(AddSeasonCommand));
                var command = (DeleteCommand<SeasonViewModel>)_dialogDeleteSeasonCommand.Command;
                command.Collection = value;
                NotifyPropertyChanged(nameof(DialogDeleteSeasonCommand));
            }
        }
        // Serialization purposes
        internal int ForegroundBinary
        {
            get
            {
                Color color = _titleFGBrush.Color;
                int argb = (color.A << 0x18) | (color.R << 0x10) | (color.G << 0x8) | color.B;
                return argb;
            }
            private set
            {
                byte a = (byte)(value >> 0x18),
                     r = (byte)(value >> 0x10),
                     g = (byte)(value >> 0x8),
                     b = (byte)value;
                Color titleColor = Color.FromArgb(a, r, g, b);
                var brushes = BrushesProvider.ProvideValue();
                for (int i = 0; i < brushes.Length; i++)
                    if (brushes[i].Color == titleColor)
                    {
                        _titleFGBrush = brushes[i];
                        NotifyPropertyChanged(nameof(Foreground));
                        break;
                    }

            }
        }

        public BingePriority Priority
        {
            get
            {
                BingePriority priority = BingePriority.Undefined;
                for (int i = 0; i < _seasons.Count; i++)
                    priority |= _seasons[i].Priority;
                return priority.GetHighestBit<BingePriority, byte>();
            }
        }
        public Rating Rating
        {
            get
            {
                Rating rating = Rating.Unrated;
                for (int i = 0; i < _seasons.Count; i++)
                    rating |= _seasons[i].Rating;
                return rating;
            }
        }

        public ICommand DialogEditPosterCommand => _dialodEditPosterCommand;

        public ICommand DialogDeleteCommand => _dialogDeleteCommand;

        public ICommand AddSeasonCommand => _addSeasonCommand;
        public ICommand DialogDeleteSeasonCommand => _dialogDeleteSeasonCommand;
        public ICommand MoveSeasonUpCommand => _moveUpSeasonCommand;
        public ICommand MoveSeasonDownCommand => _moveDownSeasonCommand;

        public MediaViewModel()
        {
            _title = nameof(Title);
            _description = string.Empty;
            _titleFGBrush = Brushes.Transparent;
            _seasons = new();

            _addSeasonCommand = new(_seasons);
            DeleteCommand<SeasonViewModel> deleteSeasonCommand = new(_seasons);
            _dialogDeleteSeasonCommand = new(deleteSeasonCommand, _dialog);
            _moveUpSeasonCommand = new(MoveSeasonUp, CanSeasonMoveUp);
            _moveDownSeasonCommand = new(MoveSeasonDown, CanSeasonMoveDown);

            DeleteSelfCommand<MediaViewModel> deleteSelfCommand = new(this);
            _dialogDeleteCommand = new(deleteSelfCommand, _dialog);

            _posterVM = new();
            ConfirmDialogService dialogPoster = new("Poster", "Done", "Cancel", GetPosterVM);
            RelayCommand editPosterCommand = new(SetPosterVM);
            _dialodEditPosterCommand = new(editPosterCommand, dialogPoster);
        }

        private PosterViewModel GetPosterVM()
        {
            _tmpPosterVM.Link = _posterVM.Link;
            return _tmpPosterVM;
        }
        private void SetPosterVM(object? _)
        {
            if (_tmpPosterVM.IsValidState)
                _posterVM.Link = _tmpPosterVM.Link;
        }
        private bool CanSeasonMoveUp(object? obj) => obj is int seasonIndex && seasonIndex > 0;
        private bool CanSeasonMoveDown(object? obj) => obj is int index && index >= 0 && index < _seasons.Count - 1;
        private void MoveSeasonUp(object? obj)
        {
            int oldIndex = (int)obj!;
            _seasons.Move(oldIndex, oldIndex - 1);
        }
        private void MoveSeasonDown(object? obj)
        {
            int oldIndex = (int)obj!;
            _seasons.Move(oldIndex, oldIndex + 1);
        }
    }
}
