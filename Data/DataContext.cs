using ChatService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatService.Data
{
    public class DataContext : DbContext
    {
        public DbSet<MessageModel> Messages { get; set; }
        public DbSet<ChatRoomModel> ChatRooms { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
    }
}
