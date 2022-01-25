using System;
using System.Collections.Generic;
using System.Timers;
using Yetibyte.Twitch.ChatNx.QueueReceiver.Models;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver.Services
{
    public class MockCommandQueueWebSocketServer : ICommandQueueWebSocketServer
    {
        private readonly List<CommandQueueItem> _itemsReceived = new List<CommandQueueItem>();
        private Timer _timer;

        public IEnumerable<CommandQueueItem> CommandQueueItemsReceived => _itemsReceived;

        public bool IsRunning { get; private set; }

        public int Port { get; set; } = 4769;

        public event EventHandler<CommandQueueItemCompletedEventArgs> CommandQueueItemCompleted;
        public event EventHandler<CommandQueueItemReceivedEventArgs> CommandQueueItemReceived;
        public event EventHandler<InvalidMessageReceivedEventArgs> InvalidMessageReceived;

        public MockCommandQueueWebSocketServer()
        {
            _timer = new Timer(5000)
            {
                AutoReset = true
            };

            _timer.Elapsed += timer_Elapsed;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CommandQueueItem commandQueueItem = new CommandQueueItem
            {
                IsCompleted = false,
                TimeStamp = DateTime.Now,
                Data = new CommandQueueItemData
                {
                    Command = "Spin",
                    Id = Guid.NewGuid().ToString(),
                    UserColorHex = "#C0FFEE",
                    UserName = "MrTest"
                }
            };

            _itemsReceived.Add(commandQueueItem);
  
            CommandQueueItemReceived?.Invoke(this, new CommandQueueItemReceivedEventArgs(commandQueueItem));

        }

        public void Start()
        {
            IsRunning = true;

            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            IsRunning = false;
        }
    }
}