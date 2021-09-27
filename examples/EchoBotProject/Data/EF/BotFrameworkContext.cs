using Microsoft.EntityFrameworkCore;

namespace EchoBotProject.Data.EF
{
    public class BotFrameworkContext : DbContext
    {
        public BotFrameworkContext(DbContextOptions<BotFrameworkContext> options) : base(options)
        { }

        public DbSet<UserModel> UserModels { get; set; }
    }
}