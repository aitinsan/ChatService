using Grpc.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService
{
    public class ChatRoom
    {
        public string Id { get; set; }
        private readonly ConcurrentDictionary<string, IServerStreamWriter<Message>> users =
            new ConcurrentDictionary<string, IServerStreamWriter<Message>>();

        public bool AddUser(string userName, IServerStreamWriter<Message> response)
        {
            return users.TryAdd(userName, response);
        }
        public ChatRoom(string Id)
        {
            this.Id = Id;
            users = new ConcurrentDictionary<string, IServerStreamWriter<Message>>();
        }

        public bool DeleteUser(string userName)
        {
            return users.TryRemove(userName, out _);
        }
        public async Task SendMessage(Message message)
        {
            foreach (var pair in users.Where(x => x.Key != message.User))
            {
                await pair.Value.WriteAsync(message);
            }
        }

        public async Task ErrorMessage(Message message)
        {
            await users[message.User].WriteAsync(message);
        }

        public async Task SendEnteringMessage(IServerStreamWriter<Message> response, Message message)
        {
            foreach(var val in users.Values)
                await val.WriteAsync(message);
        }
    }
}
