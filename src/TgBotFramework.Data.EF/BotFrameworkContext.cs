using Microsoft.EntityFrameworkCore;
using TgBotFramework.Models;

namespace TgBotFramework.Data.EF
{
    public class BotFrameworkContext : DbContext
    {
        public BotFrameworkContext(DbContextOptions<BotFrameworkContext> options) : base(options)
       // public BotFrameworkContext(DbContextOptions<BotFrameworkContext> options) : base(options)
        { }

        public DbSet<UserModel> UserModels { get; set; }
    }
}