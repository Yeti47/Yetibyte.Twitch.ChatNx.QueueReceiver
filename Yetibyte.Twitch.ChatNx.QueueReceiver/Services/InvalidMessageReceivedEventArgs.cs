using System;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver.Services
{
    public class InvalidMessageReceivedEventArgs : EventArgs
    {
        public string Message { get; }

        public InvalidMessageReceivedEventArgs(string message)
        {
            Message = message ?? string.Empty;
        }

    }
}
