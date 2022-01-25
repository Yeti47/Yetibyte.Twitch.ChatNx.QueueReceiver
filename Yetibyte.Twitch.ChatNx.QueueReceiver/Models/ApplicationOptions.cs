using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver.Models
{
    public class ApplicationOptions : INotifyPropertyChanged
    {
        private SolidColorBrush _backgroundColor;
        private SolidColorBrush _queueItemTextColor;
        private SolidColorBrush _historyItemTextColor;
        private SolidColorBrush _headerTextColor;
        private SolidColorBrush _currentItemBorderColor;
        private SolidColorBrush _dividerColor;

        private int _headerTextSize;
        private int _queueItemTextSize;
        private int _historyItemTextSize;

        private float _timeStampScalePercentage;

        private int _port;

        public int Port
        {
            get { return _port; }
            set { _port = value; OnPropertyChanged(); }
        }

        public float TimeStampScalePercentage
        {
            get { return _timeStampScalePercentage; }
            set { 
                _timeStampScalePercentage = value; 
                OnPropertyChanged();
                OnPropertyChanged(nameof(QueueItemTimeStampTextSize));
                OnPropertyChanged(nameof(HistoryItemTimeStampTextSize));
            }
        }

        public int QueueItemTimeStampTextSize => (int)Math.Round(_queueItemTextSize * (_timeStampScalePercentage / 100f));
        public int HistoryItemTimeStampTextSize => (int)Math.Round(_historyItemTextSize * (_timeStampScalePercentage / 100f));


        public int HeaderTextSize
        {
            get { return _headerTextSize; }
            set { _headerTextSize = value; OnPropertyChanged(); }
        }

        public int QueueItemTextSize
        {
            get { return _queueItemTextSize; }
            set { 
                _queueItemTextSize = value; 
                OnPropertyChanged();
                OnPropertyChanged(nameof(QueueItemTimeStampTextSize));
            }
        }

        public int HistoryItemTextSize
        {
            get { return _historyItemTextSize; }
            set { 
                _historyItemTextSize = value; 
                OnPropertyChanged();
                OnPropertyChanged(nameof(HistoryItemTimeStampTextSize));
            }
        }

        public SolidColorBrush DividerColor
        {
            get { return _dividerColor; }
            set { _dividerColor = value; OnPropertyChanged(); }
        }

        public SolidColorBrush CurrentItemBorderColor
        {
            get { return _currentItemBorderColor; }
            set { _currentItemBorderColor = value; OnPropertyChanged(); }
        }

        public SolidColorBrush HeaderTextColor
        {
            get { return _headerTextColor; }
            set { _headerTextColor = value; OnPropertyChanged(); }
        }

        public SolidColorBrush HistoryItemTextColor
        {
            get { return _historyItemTextColor; }
            set { _historyItemTextColor = value; OnPropertyChanged(); }
        }

        public SolidColorBrush BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; OnPropertyChanged(); }
        }

        public SolidColorBrush QueueItemTextColor
        {
            get => _queueItemTextColor;
            set
            {
                _queueItemTextColor = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Copy(ApplicationOptions otherOptions)
        {
            _backgroundColor = otherOptions._backgroundColor;
            _queueItemTextColor = otherOptions._queueItemTextColor;
            _historyItemTextColor = otherOptions._historyItemTextColor;
            _headerTextColor = otherOptions._headerTextColor;
            _currentItemBorderColor = otherOptions._currentItemBorderColor;
            _dividerColor = otherOptions._dividerColor;

            _headerTextSize = otherOptions._headerTextSize;
            _queueItemTextSize = otherOptions._queueItemTextSize;
            _historyItemTextSize = otherOptions._historyItemTextSize;

            _timeStampScalePercentage = otherOptions._timeStampScalePercentage;

            _port = otherOptions._port;

            OnPropertyChanged(null);
        }

        public static ApplicationOptions CreateDefault()
        {
            return new ApplicationOptions
            {
                BackgroundColor = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)),
                QueueItemTextColor = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
                HistoryItemTextColor = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
                HeaderTextColor = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
                CurrentItemBorderColor = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
                DividerColor = new SolidColorBrush(Color.FromArgb(255, 120, 120, 120)),
                HeaderTextSize = 22,
                QueueItemTextSize = 30,
                HistoryItemTextSize = 30,
                TimeStampScalePercentage = 50f,
                Port = 4769
            };
        }
    }
}
