using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Text.Json;
using System.Text.Json.Serialization;
using Yetibyte.Twitch.ChatNx.QueueReceiver.Models;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver.Services
{
    public class CommandQueueWebSocketBehavior : WebSocketBehavior
    {
        private readonly Func<QueueStatus> _queueStatusCallback;

        public CommandQueueWebSocketBehavior(Func<QueueStatus> queueStatusCallback)
        {
            _queueStatusCallback = queueStatusCallback ?? (new Func<QueueStatus>(() => default(QueueStatus)));
        }

        public event EventHandler<CommandQueueItemReceivedEventArgs> CommandQueueItemReceived;
        public event EventHandler<CommandQueueItemCompletedEventArgs> CommandQueueItemCompleted;
        public event EventHandler<InvalidMessageReceivedEventArgs> InvalidMessageReceived;
        public event EventHandler ClearRequestReceived;

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {   
                CommandQueueRequest request = JsonSerializer.Deserialize<CommandQueueRequest>(e.Data);
                
                string action = request?.Action ?? string.Empty;

                if (action.Equals(CommandQueueRequest.ACTION_ADD, StringComparison.OrdinalIgnoreCase)) {

                    OnCommandQueueItemReceived(new CommandQueueItem
                    {
                        Data = request.ItemData,
                        TimeStamp = DateTime.Now,
                        IsCompleted = false
                    });

                }
                else if(action.Equals(CommandQueueRequest.ACTION_COMPLETE))
                {
                    OnCommandQueueItemCompleted(request.ItemData.Id);
                }
                else if(action.Equals(CommandQueueRequest.ACTION_CHECK))
                {
                    QueueStatus queueStatus = _queueStatusCallback.Invoke();

                    string queueStatusJson = JsonSerializer.Serialize(queueStatus);

                    Send(queueStatusJson);
                }
                else if (action.Equals(CommandQueueRequest.ACTION_CLEAR))
                {
                    OnClearRequestReceived();
                }
                else
                {
                    OnInvalidMessageReceived(e?.Data);
                }

            }
            catch 
            {
                OnInvalidMessageReceived(e?.Data);
            } 
        }

        protected virtual void OnCommandQueueItemReceived(CommandQueueItem commandQueueItem)
        {
            var handler = CommandQueueItemReceived;
            handler?.Invoke(this, new CommandQueueItemReceivedEventArgs(commandQueueItem));
        }

        protected virtual void OnCommandQueueItemCompleted(string itemId)
        {
            var handler = CommandQueueItemCompleted;
            handler?.Invoke(this, new CommandQueueItemCompletedEventArgs(itemId));
        }

        protected virtual void OnInvalidMessageReceived(string message)
        {
            var handler = InvalidMessageReceived;
            handler?.Invoke(this, new InvalidMessageReceivedEventArgs(message));
        }

        protected virtual void OnClearRequestReceived()
        {
            var handler = ClearRequestReceived;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
