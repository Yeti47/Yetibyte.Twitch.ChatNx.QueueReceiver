using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver.Models
{
    [Serializable]
    public class CommandQueueRequest
    {
        public const string ACTION_ADD = "ADD";
        public const string ACTION_COMPLETE = "COMPLETE";
        public const string ACTION_CHECK = "CHECK";

        public string Action { get; init; }
        public CommandQueueItemData ItemData { get; init; }
    }
}
