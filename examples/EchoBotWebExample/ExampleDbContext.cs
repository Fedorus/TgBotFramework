using Microsoft.EntityFrameworkCore;
using TgBotFramework.Data.EF;

namespace EchoBotWebExample
{
    public class ExampleDbContext : BotFrameworkContext
    {
        public ExampleDbContext(DbContextOptions<BotFrameworkContext> options) : base(options)
        {
        }
    }
}