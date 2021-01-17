using ChatService.Models;
using ChatService.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService
{
    public class ChatRoomService
    {
        private readonly MessagesRepository _messages;
        private ILogger<ChatRoomService> _logger;
        private static readonly ConcurrentBag<ChatRoom> chatRooms = new ConcurrentBag<ChatRoom>();
        public ChatRoomService(MessagesRepository messages, ILogger<ChatRoomService> logger)
        {
            _messages = messages;
            _logger = logger;
        }
        public async Task Join(string userName, ChatRoomInfo chatRoom, IServerStreamWriter<Message> response)
        {
            if(!chatRooms.Any(c => c.Id.Equals(chatRoom.Id)))
            chatRooms.Add(new ChatRoom(chatRoom.Id));
            var room = chatRooms.First(c => c.Id.Equals(chatRoom.Id));

            if (room.AddUser(userName, response))
            {
                var chatRoomId = Guid.Parse(chatRoom.Id);
                if (!await _messages.HasUser(chatRoomId, userName))
                {
                    _logger.LogInformation($"Creating entering message for user {userName} on chat room {chatRoom.Name}");
                    await room.SendEnteringMessage(response,
                        new Message { Text = " has entered the chat!", User = userName, ChatRoomId = chatRoom.Id });
                }
                else _messages.HistoryMessages(Guid.Parse(chatRoom.Id)).Result.ForEach(m => response.WriteAsync(m));
            }
        }


        public void Remove(Guid chatRoomId, string name)
        {
            var room = chatRooms.First(c => c.Id.Equals(chatRoomId));
            room.DeleteUser(name);
        }

        public async Task BroadcastMessageAsync(Message message) => await BroadcastMessage(message);

        private async Task BroadcastMessage(Message message)
        {
            var messageModel = new MessageModel { User = message.User, Text = message.Text,
                ChatRoomId = Guid.Parse(message.ChatRoomId) };
                var room = chatRooms.First(c => c.Id.Equals(message.ChatRoomId));
            if(await _messages.AddMessage(messageModel))
            {
                await room.SendMessage(message);
            }
            else
            {
                message.Text = "Failed to add message";
                _logger.LogError(message.Text);
                await room.ErrorMessage(message);
            } 
        }

       /* private async Task<KeyValuePair<string, IServerStreamWriter<Message>>?> SendMessageToSubscriber(
            KeyValuePair<string, IServerStreamWriter<Message>> pair, Message message)
        {
            try
            {
                await pair.Value.WriteAsync(message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return pair;
            }
        }*/
    }

}
