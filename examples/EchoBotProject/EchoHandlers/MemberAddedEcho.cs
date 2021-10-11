using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TgBotFramework;

namespace EchoBotProject.EchoHandlers
{
    public class MemberAddedEcho : IUpdateHandler<BotExampleContext>
    {
        private readonly ILogger<MemberAddedEcho> _logger;

        public MemberAddedEcho(ILogger<MemberAddedEcho> logger)
        {
            _logger = logger;
        }
        
        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            var item = context.Update.Message;
            _logger.LogInformation("UserOrBot {0} was added to chat {1} by {2}", item.NewChatMembers[0], item.Chat, item.From);
        }
    }
}