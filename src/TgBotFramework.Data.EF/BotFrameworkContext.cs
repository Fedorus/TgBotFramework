using Microsoft.EntityFrameworkCore;
using TgBotFramework.Data.EF.Models;

namespace TgBotFramework.Data.EF
{
    public class BotFrameworkContext : DbContext
    {
        public BotFrameworkContext(DbContextOptions<BotFrameworkContext> options) : base(options)
        {
        }

        public DbSet<Channel> Channels { get; set; }
        public DbSet<GroupChat> GroupChats { get; set; }
        public DbSet<ChatMemberStatus> ChatMemberStatus { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Update> Updates { get; set; }
        public DbSet<User> Users { get; set; }
    }
}