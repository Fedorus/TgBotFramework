using Microsoft.Extensions.Options;
using TgBotFramework;

namespace Bandersnatch.Bot
{
    public class BandersnatchBot : BaseBot
    {   
        public BandersnatchBot(IOptions<BotSettings> options) : base(options)
        {
        }
    }
}