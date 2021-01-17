using ChatService.Data;
using ChatService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService.Repositories
{
    public class MessagesRepository
    {
        private readonly DataContext _context;
        public MessagesRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<List<Message>> HistoryMessages()
        {
                return await _context.Messages.Select(m => new Message { Text = m.Text, User = m.User, ChatRoomId = m.ChatRoomId.ToString() }).ToListAsync();
        }
        public async Task<List<Message>> HistoryMessages(Guid chatRoomId)
        {
            return await _context.Messages.Where(m => m.ChatRoomId.Equals(chatRoomId)).
                Select(m => new Message { Text = m.Text, User = m.User, ChatRoomId = m.ChatRoomId.ToString() }).ToListAsync();
        }
        public async Task<bool> AddMessage(MessageModel message)
        {
            _context.Messages.Add(message);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> HasUser(Guid chatRoomId, string userName) {
            return await _context.Messages.AnyAsync(m => m.User.Equals(userName) && m.ChatRoomId.Equals(chatRoomId));
        }
    }
}
