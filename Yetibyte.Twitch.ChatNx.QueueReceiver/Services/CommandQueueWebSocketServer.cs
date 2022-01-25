using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WebSocketSharp.Server;
using Yetibyte.Twitch.ChatNx.QueueReceiver.Models;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver.Services
{
    public class CommandQueueWebSocketServer : ICommandQueueWebSocketServer
    {
        public const int DEFAULT_PORT = 4769;
        public const string QUEUE_ENDPOINT_PATH = "/Queue";
        public const string LOG_FILE_PATH = @"WebSocket.log";

        private WebSocketServer _webSocketServer;
        private CommandQueueWebSocketBehavior _queueBehavior;
        private readonly List<CommandQueueItem> _commandQueueItemsReceived = new List<CommandQueueItem>();
        private readonly Func<QueueStatus> _queueStatusCallback;

        public int Port { get; set; }
        public bool IsRunning { get; private set; }

        public IEnumerable<CommandQueueItem> CommandQueueItemsReceived => new ReadOnlyCollection<CommandQueueItem>(_commandQueueItemsReceived);

        public event EventHandler<CommandQueueItemReceivedEventArgs> CommandQueueItemReceived;
        public event EventHandler<CommandQueueItemCompletedEventArgs> CommandQueueItemCompleted;
        public event EventHandler<InvalidMessageReceivedEventArgs> InvalidMessageReceived;

        public CommandQueueWebSocketServer(Func<QueueStatus> queueStatusCallback, int port = DEFAULT_PORT)
        {
            _queueStatusCallback = queueStatusCallback ?? (new Func<QueueStatus>(() => default(QueueStatus)));

            Port = port;
            IsRunning = false;
        }

        private void QueueBehavior_InvalidMessageReceived(object sender, InvalidMessageReceivedEventArgs e)
        {
            OnInvalidMessageReceived(e?.Message);
        }

        private void QueueBehavior_CommandQueueItemReceived(object sender, CommandQueueItemReceivedEventArgs e)
        {
            OnCommandQueueItemReceived(e.CommandQueueItem);
        }

        private void QueueBehavior_CommandQueueItemCompleted(object sender, CommandQueueItemCompletedEventArgs e)
        {
            OnCommandQueueItemCompleted(e.ItemId);
        }

        public void Start()
        {
            _webSocketServer?.Stop();
            _webSocketServer = new WebSocketServer(Port)
            {
                KeepClean = false // KeepClean property MUST be set to false. It defaults to true, but there is a
                                  // bug in the websocket-sharp library that causes the NetworkStream to be disposed which leads to frequent disconnects
            };
            _webSocketServer.Log.Level = WebSocketSharp.LogLevel.Info;
            _webSocketServer.Log.File = LOG_FILE_PATH;

            _webSocketServer.AddWebSocketService(QUEUE_ENDPOINT_PATH, () =>
            {
                CommandQueueWebSocketBehavior queueBehavior = new CommandQueueWebSocketBehavior(_queueStatusCallback)
                {
                    Protocol = "socketcrutch"
                };
                queueBehavior.CommandQueueItemReceived += QueueBehavior_CommandQueueItemReceived;
                queueBehavior.InvalidMessageReceived += QueueBehavior_InvalidMessageReceived;
                queueBehavior.CommandQueueItemCompleted += QueueBehavior_CommandQueueItemCompleted;

                _queueBehavior = queueBehavior;

                return queueBehavior;
            });

            _webSocketServer.Start();

            IsRunning = true;
        }



        public void Stop()
        {
            if (_queueBehavior is not null)
            {
                _queueBehavior.CommandQueueItemReceived -= QueueBehavior_CommandQueueItemReceived;
                _queueBehavior.InvalidMessageReceived -= QueueBehavior_InvalidMessageReceived;
                _queueBehavior.CommandQueueItemCompleted -= QueueBehavior_CommandQueueItemCompleted;
            }

            _webSocketServer?.Stop();

            IsRunning = false;
        }

        protected virtual void OnCommandQueueItemReceived(CommandQueueItem commandQueueItem)
        {
            _commandQueueItemsReceived.Add(commandQueueItem);

            var handler = CommandQueueItemReceived;
            handler?.Invoke(this, new CommandQueueItemReceivedEventArgs(commandQueueItem));
        }

        protected virtual void OnCommandQueueItemCompleted(string itemId)
        {
            foreach(var item in _commandQueueItemsReceived.Where(q => q.Data?.Id == (itemId ?? string.Empty))) {

                item.IsCompleted = true;

            }

            var handler = CommandQueueItemCompleted;
            handler?.Invoke(this, new CommandQueueItemCompletedEventArgs(itemId));
        }

        protected virtual void OnInvalidMessageReceived(string message)
        {
            var handler = InvalidMessageReceived;
            handler?.Invoke(this, new InvalidMessageReceivedEventArgs(message));
        }
    }
}
