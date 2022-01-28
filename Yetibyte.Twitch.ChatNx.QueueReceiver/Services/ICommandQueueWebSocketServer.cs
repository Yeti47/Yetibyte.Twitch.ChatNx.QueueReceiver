using System;
using System.Collections.Generic;
using Yetibyte.Twitch.ChatNx.QueueReceiver.Models;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver.Services
{
    public interface ICommandQueueWebSocketServer
    {
        IEnumerable<CommandQueueItem> CommandQueueItemsReceived { get; }
        bool IsRunning { get; }
        int Port { get; set; }

        event EventHandler<CommandQueueItemCompletedEventArgs> CommandQueueItemCompleted;
        event EventHandler<CommandQueueItemReceivedEventArgs> CommandQueueItemReceived;
        event EventHandler<InvalidMessageReceivedEventArgs> InvalidMessageReceived;
        event EventHandler ClearRequestReceived;

        void Start();
        void Stop();
    }
}