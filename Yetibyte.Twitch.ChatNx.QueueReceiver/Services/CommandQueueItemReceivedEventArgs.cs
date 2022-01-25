using System;
using Yetibyte.Twitch.ChatNx.QueueReceiver.Models;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver.Services
{
    public class CommandQueueItemReceivedEventArgs : EventArgs
    {
        public CommandQueueItem CommandQueueItem { get; }

        public CommandQueueItemReceivedEventArgs(CommandQueueItem commandQueueItem)
        {
            CommandQueueItem = commandQueueItem ?? throw new ArgumentNullException(nameof(commandQueueItem));
        }
    }
}
