using ChatService.Helpers;
using ChatService.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
namespace ChatService.Services
{
    public class ChattingService : ChatService.ChatServiceBase
    {
        private readonly EventHandler<ChatEventArgs> chatHandler = delegate { };
        private readonly ChatRoomService _chatRoomService;
        private readonly ChatroomsRepository _chatRooms;
        private readonly ILogger<ChattingService> _logger;
        public ChattingService(ChatRoomService chatRoomService, ChatroomsRepository chatRooms, ILogger<ChattingService> logger)
        {
            _chatRoomService = chatRoomService;
            _chatRooms = chatRooms;
            _logger = logger;
            chatHandler += OnRoomJoined;
            chatHandler += OnMessageReceived;
        }

        private async void OnMessageReceived(object sender, ChatEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Message.Text))
            {
                _logger.LogInformation($"Received message from {e.Sender}: Received date: {e.ReceivedDate.ToShortDateString()}");
                _logger.LogInformation($"Sending message to chat room {e.ChatRoomId}...");
                await _chatRoomService.BroadcastMessageAsync(e.Message);
            }
        }

        private async void OnRoomJoined(object sender, ChatEventArgs e)
        {
            if (!e.ChatRoomId.Equals(Guid.Empty))
            {
                _logger.LogInformation($"Trying to connect to chat room {e.ChatRoomId}: {e.ReceivedDate}");
                await JoinChatRoom(new ChatRoomJoin() { User = e.Sender, ChatRoomId = e.ChatRoomId.ToString() }, e.MessageStream);
            }
        }

        public override async Task JoinAndWriteMessage(IAsyncStreamReader<Message> requestStream, IServerStreamWriter<Message> responseStream, ServerCallContext context)
        {
            if (!await requestStream.MoveNext()) return;
            do
            {
                var eventArgs = new ChatEventArgs(requestStream.Current.User, requestStream.Current, responseStream);
                chatHandler.Invoke(null, eventArgs);
            } while (await requestStream.MoveNext());

            _chatRoomService.Remove(Guid.Parse(requestStream.Current.ChatRoomId), context.Peer);
        }
        private async Task JoinChatRoom(ChatRoomJoin request, IServerStreamWriter<Message> responseStream)
        {
            Guid chatRoomId = Guid.Parse(request.ChatRoomId);
            var chatRoom = await _chatRooms.GetById(chatRoomId);
            if (chatRoom != null)
                await _chatRoomService.Join(request.User, chatRoom, responseStream);
        }

        public override async Task<ChatRooms> GetChatRooms(Lookup request, ServerCallContext context)
        {
            var result = new ChatRooms();
            var allChatRooms = await _chatRooms.GetAll();
            result.ChatRooms_.AddRange(allChatRooms);
            return result;
        }
    }
}
