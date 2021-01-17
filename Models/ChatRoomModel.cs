using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService.Models
{
    public class ChatRoomModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<MessageModel> Messages { get; set; }
    }
}
