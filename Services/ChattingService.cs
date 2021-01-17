using ChatService.Repositories;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ChatService.Services
{
    public class ChattingService : ChatService.ChatServiceBase
    {
        private readonly ChatRoomService _chatRoomService;
        private readonly ChatroomsRepository _chatRooms;
        public ChattingService(ChatRoomService chatRoomService, ChatroomsRepository chatRooms)
        {
            _chatRoomService = chatRoomService;
            _chatRooms = chatRooms;
        }

        public override async Task JoinAndWriteMessage(IAsyncStreamReader<Message> requestStream, IServerStreamWriter<Message> responseStream, ServerCallContext context)
        {
            if (!await requestStream.MoveNext()) return;
            var chatJoin = new ChatRoomJoin();
            do
            {
                chatJoin.User = requestStream.Current.User;
                chatJoin.ChatRoomId = requestStream.Current.ChatRoomId;
                await JoinChatRoom(chatJoin, responseStream);

                if(!string.IsNullOrEmpty(requestStream.Current.Text))
                await _chatRoomService.BroadcastMessageAsync(requestStream.Current);
            } while (await requestStream.MoveNext());

            _chatRoomService.Remove(Guid.Parse(chatJoin.ChatRoomId), context.Peer);
        }
        private async Task JoinChatRoom(ChatRoomJoin request, IServerStreamWriter<Message> responseStream)
        {
            Guid chatRoomId = Guid.Parse(request.ChatRoomId);
            var chatRoom = await _chatRooms.GetById(chatRoomId);
            if(chatRoom != null)
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
