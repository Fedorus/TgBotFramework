using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;

namespace EchoBotProject.Handlers
{
    public class PublicChatEcho : IUpdateHandler<BotExampleContext>
    {
        private readonly ILogger<PublicChatEcho> _logger;

        public PublicChatEcho(ILogger<PublicChatEcho> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            _logger.LogInformation("This update {0} was from public chat {1}", context.Update.Id, context.Update.GetChat());
        }
    }
}