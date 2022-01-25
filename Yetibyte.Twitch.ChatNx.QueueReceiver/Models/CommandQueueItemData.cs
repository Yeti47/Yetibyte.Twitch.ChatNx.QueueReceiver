using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver.Models
{
    [Serializable]
    public record CommandQueueItemData
    {
        public string Id { get; init; }
        public string UserName { get; init; }
        public string UserColorHex { get; init; }

        public string Command { get; init; }
    }
}