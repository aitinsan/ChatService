using ChatService.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService.Repositories
{
    public class ChatroomsRepository
    {
        private readonly DataContext _context;

        public ChatroomsRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<ChatRoomInfo>> GetAll()
        {
            return await _context.ChatRooms.Select(c => new ChatRoomInfo { Id = c.Id.ToString(), Name = c.Name }).ToListAsync();
        }

        public async Task<ChatRoomInfo> GetById(Guid id)
        {
            var chatRoom = await _context.ChatRooms.FindAsync(id);
            return new ChatRoomInfo { Id = chatRoom.Id.ToString(), Name = chatRoom.Name };
        }
    }
}
