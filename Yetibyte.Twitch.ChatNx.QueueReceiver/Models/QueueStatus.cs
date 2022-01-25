using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver.Models
{
    [Serializable]
    public class QueueStatus
    {
        public int QueueItemCount { get; init; }
        public int HistoryItemCount { get; init; }

        public string[] QueueItemIds { get; init; }

    }
}
