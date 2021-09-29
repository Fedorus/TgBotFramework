using EchoBotProject.Data;
using EchoBotProject.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace EchoBotProject
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
        
        public DbSet<UserModel> UserModels { get; set; }
    }
}