using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Yetibyte.Twitch.ChatNx.QueueReceiver.Models;
using Yetibyte.Twitch.ChatNx.QueueReceiver.Services;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver.Mvvm.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public const string CLEAR_PARAMETER_HISTORY = "HISTORY";
        public const string CLEAR_PARAMETER_QUEUE = "QUEUE";

        public const int AUTO_COMPLETE_ITEM_MILLISECS = 1000 * 60 * 10; // 10 mins

        private readonly ICommandQueueWebSocketServer _commandQueueWebSocketServer;
        private readonly OptionService _optionService;
        private readonly ObservableCollection<CommandQueueItem> _queuedItems = new ObservableCollection<CommandQueueItem>();
        private readonly ObservableCollection<CommandQueueItem> _historyItems = new ObservableCollection<CommandQueueItem>();
        private CommandQueueItem _currentItem = null;

        private readonly DelegateCommand _startCommand;
        private readonly DelegateCommand _stopCommand;
        private readonly DelegateCommand _clearCommand;

        private bool _isPortEnabled;
        private Visibility _optionsVisibility;

        public Visibility OptionsVisibility
        {
            get { return _optionsVisibility; }
            set { _optionsVisibility = value; OnPropertyChanged(); }
        }

        public int QueueItemCount => _queuedItems.Count + (CurrentItem is not null ? 1 : 0);
        public int HistoryItemCount => _historyItems.Count;

        public int? Port => Options.Port;

        public bool IsPortEnabled
        {
            get => _isPortEnabled; private set
            {
                _isPortEnabled = value;
                OnPropertyChanged();
            }
        }

        public ICommand StartCommand => _startCommand;
        public ICommand StopCommand => _stopCommand;
        public ICommand ClearCommand => _clearCommand;

        public IEnumerable<CommandQueueItem> CommandQueueItems => _queuedItems;
        public IEnumerable<CommandQueueItem> HistoryItems => _historyItems;

        public CommandQueueItem CurrentItem
        {
            get => _currentItem;
            set
            {
                _currentItem = value;

                if (value != null)
                {

                    if (!value.IsCompleted)
                    {
                        // Automatically set the item to complete after a certain amount of time
                        Task.Delay(AUTO_COMPLETE_ITEM_MILLISECS).ContinueWith(_ => {

                            value.IsCompleted = true;
                        });
                    }
                    else // Item is already complete, so move to history after short visible delay.
                    {
                        Task.Delay(500).ContinueWith(_ => MoveCurrentItemToHistory());
                    }
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(QueueItemCount));
            }
        }

        public ApplicationOptions Options { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel(ICommandQueueWebSocketServer commandQueueWebSocketServer, OptionService optionService)
        {
            _optionService = optionService ?? throw new ArgumentNullException(nameof(optionService));

            IsPortEnabled = true;
            OptionsVisibility = Visibility.Collapsed;

            _commandQueueWebSocketServer = commandQueueWebSocketServer ?? throw new ArgumentNullException(nameof(commandQueueWebSocketServer));
            _commandQueueWebSocketServer.CommandQueueItemReceived += commandQueueWebSocketServer_CommandQueueItemReceived;
            _commandQueueWebSocketServer.ClearRequestReceived += commandQueueWebSocketServer_ClearRequestReceived;

            _startCommand = new DelegateCommand(ExecuteStartCommand, CanExecuteStartCommand);
            _stopCommand = new DelegateCommand(ExecuteStopCommand, CanExecuteStopCommand);
            _clearCommand = new DelegateCommand(ExecuteClearCommand, DelegateCommand.AlwaysTruePredicate);

            Options = _optionService.LoadApplicationOptions();

            _queuedItems.CollectionChanged += queuedItems_CollectionChanged;
            _historyItems.CollectionChanged += historyItems_CollectionChanged;
        }

        private void commandQueueWebSocketServer_ClearRequestReceived(object sender, EventArgs e)
        {
            ExecuteClearCommand(CLEAR_PARAMETER_QUEUE);
        }

        private void historyItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HistoryItemCount));
        }

        private void queuedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(QueueItemCount));
        }

        private void commandQueueWebSocketServer_CommandQueueItemReceived(object sender, CommandQueueItemReceivedEventArgs e)
        {
            if(CurrentItem is null)
            {
                CurrentItem = e.CommandQueueItem;
                CurrentItem.PropertyChanged += CurrentItem_PropertyChanged;
            }
            else
            {
                // We need to add the item using the dispatcher, since the event handler is called from another thread.
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    _queuedItems.Insert(0, e.CommandQueueItem);
                });
            }
        }

        private void CurrentItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (CurrentItem.IsCompleted)
            {
                MoveCurrentItemToHistory();
            }
        }

        private void MoveCurrentItemToHistory()
        {
            CurrentItem.PropertyChanged -= CurrentItem_PropertyChanged;

            System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
            {
                _historyItems.Add(CurrentItem);
            });

            if (_queuedItems.Any())
            {
                var lastItem = _queuedItems.Last();

                System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
                {
                    _queuedItems.Remove(lastItem);
                });

                CurrentItem = lastItem;
                CurrentItem.PropertyChanged += CurrentItem_PropertyChanged;
            }
            else
            {
                CurrentItem = null;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ExecuteStartCommand(object parameter)
        {
            _commandQueueWebSocketServer.Port = Port ?? CommandQueueWebSocketServer.DEFAULT_PORT;
            _commandQueueWebSocketServer.Start();

            _stopCommand.RaiseCanExecuteChangedEvent();
            _startCommand.RaiseCanExecuteChangedEvent();

            IsPortEnabled = false;
        }

        private bool CanExecuteStartCommand(object parameter) => !_commandQueueWebSocketServer.IsRunning;

        private void ExecuteStopCommand(object parameter)
        {
            _commandQueueWebSocketServer.Stop();

            _stopCommand.RaiseCanExecuteChangedEvent();
            _startCommand.RaiseCanExecuteChangedEvent();

            IsPortEnabled = true;
        }

        private void ExecuteClearCommand(object parameter)
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                bool wasQueueCleared = false;

                if (parameter?.ToString() == CLEAR_PARAMETER_QUEUE)
                {
                    _queuedItems.Clear();

                    wasQueueCleared = true;
                }
                else if(parameter?.ToString() == CLEAR_PARAMETER_HISTORY)
                {
                    _historyItems.Clear();
                }
                else
                {
                    _queuedItems.Clear();
                    _historyItems.Clear();

                    wasQueueCleared = true;
                }

                if (wasQueueCleared)
                {
                    if(CurrentItem is CommandQueueItem currentItem) // assigning to local var for thread safety)
                    {
                        currentItem.PropertyChanged -= CurrentItem_PropertyChanged;
                    }

                    CurrentItem = null;
                }   
                
            });
        }

        private bool CanExecuteStopCommand(object parameter) => _commandQueueWebSocketServer.IsRunning;

    }


}
