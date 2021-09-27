using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;

namespace EchoBotProject.EchoHandlers
{
    public class BotWasBlockedUnblockedOrAdded : IUpdateHandler<BotExampleContext>
    {
        private readonly ILogger<BotWasBlockedUnblockedOrAdded> _logger;

        public BotWasBlockedUnblockedOrAdded(ILogger<BotWasBlockedUnblockedOrAdded> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update {0}, user {1} change bot status {2} -> {3} in private chat", context.Update.Id,
                context.Update.GetSender(), context.Update.MyChatMember.OldChatMember.Status,
                context.Update.MyChatMember.NewChatMember.Status);
        }
    }
}