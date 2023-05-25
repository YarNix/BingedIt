using System;
using BingedIt.Common;

namespace BingedIt.ViewModels
{
    public sealed class SeasonViewModel : BaseViewModel
    {
        private string _link = string.Empty;
        private string _comment = string.Empty;
        private Status _status = Status.Unplanned;
        private Rating _rating = Rating.Unrated;
        private int _progress = 0;
        private bool _useReminder = false;
        private DateTime? _remindDate = null;
        private DateTime? _planDate = null;

        public string Link
        {
            get => _link;
            set
            {
                string val = value ?? string.Empty;
                if (val == _link) return;
                _link = val;
                NotifyPropertyChanged();
            }
        }
        public string Comment
        {
            get => _comment;
            set
            {
                if (value == _comment) return;
                _comment = value;
                NotifyPropertyChanged();
            }
        }
        public Status Status
        {
            get => _status;
            set
            {
                if (value == _status) return;
                _status = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Priority));
            }
        }
        public Rating Rating
        {
            get => _rating;
            set
            {
                if (_rating == value) return;
                _rating = value;
                NotifyPropertyChanged();
            }
        }
        // Using double instead int for the sake of being compatible with the NumberBox control, coerce the value if needed
        public double Progress
        {
            get => _progress;
            set
            {
                int newValue = double.IsNormal(value) ? (int)double.Truncate(value) : 0;
                if (newValue == _progress && value == _progress) return;
                _progress = newValue;
                NotifyPropertyChanged();
            }
        }
        public bool UseReminder
        {
            get => _useReminder;
            set
            {
                if (value == _useReminder) return;
                _useReminder = value;
                NotifyPropertyChanged();
                if (_remindDate.HasValue) NotifyPropertyChanged(nameof(Priority));
            }
        }
        public DateTime? RemindDate
        {
            get => _remindDate;
            set
            {
                if (value == _remindDate) return;
                _remindDate = value;
                NotifyPropertyChanged();
                if (_useReminder) NotifyPropertyChanged(nameof(Priority));
            }
        }
        public DateTime? PlanDate
        {
            get => _planDate;
            set
            {
                if (value == _planDate) return;
                _planDate = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Priority));
            }
        }

        public BingePriority Priority
        {
            get
            {
                BingePriority priority;
                switch (_status)
                {
                    case Status.Planning:
                        if (_planDate.HasValue && _planDate.Value <= DateTime.Today)
                            priority = BingePriority.MustBinge;
                        else
                            priority = BingePriority.WillBinge;
                        break;
                    case Status.Binging:
                        if (_useReminder && _remindDate.HasValue && _remindDate.Value <= DateTime.Today)
                            priority = BingePriority.MustBinge;
                        else
                            priority = BingePriority.BingeNext;
                        break;
                    case Status.OnHold:
                        priority = BingePriority.WillBinge;
                        break;
                    case Status.Unplanned:
                        priority = BingePriority.WontBinge;
                        break;
                    case Status.Binged:
                        priority = BingePriority.DoneBinge;
                        break;
                    default:
                        priority = BingePriority.Undefined;
                        break;
                }
                return priority;
            }
        }
    }
}
