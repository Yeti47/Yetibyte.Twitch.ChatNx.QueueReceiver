using System;
using System.ComponentModel;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver.Models
{
    public class CommandQueueItem : INotifyPropertyChanged
    {
        private bool _isCompleted;

        public CommandQueueItemData Data { get; init; }
        public DateTime TimeStamp { get; init; }
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                _isCompleted = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

    }
}