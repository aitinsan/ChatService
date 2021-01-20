using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService.Helpers
{
    public class ChatEventArgs : EventArgs
    {
        public string Sender { get; set; }
        public Message Message { get; set; }
        public DateTime ReceivedDate { get; set; }
        public Guid ChatRoomId { get; set; }
        public IServerStreamWriter<Message> MessageStream { get; set; }
        public ChatEventArgs(string sender, Message message, IServerStreamWriter<Message> messageStream)
        {
            Sender = sender;
            Message = message;
            ChatRoomId = Guid.Parse(message.ChatRoomId);
            MessageStream = messageStream;
            ReceivedDate = DateTime.Now;
        }
    }
}