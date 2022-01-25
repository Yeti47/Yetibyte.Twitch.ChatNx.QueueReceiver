using System;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver.Services
{
    public class CommandQueueItemCompletedEventArgs : EventArgs {

        public string ItemId { get; private set; }

        public CommandQueueItemCompletedEventArgs(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                throw new ArgumentException($"'{nameof(itemId)}' cannot be null or whitespace.", nameof(itemId));
            }

            ItemId = itemId;
        }

    }
}
