using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService.Models
{
    public class MessageModel
    {
        public Guid Id { get; set; }
        public string User { get; set; }
        public string Text { get; set; }
        public Guid ChatRoomId { get; set; }
        public ChatRoomModel ChatRoom { get; set; }
    }
}
