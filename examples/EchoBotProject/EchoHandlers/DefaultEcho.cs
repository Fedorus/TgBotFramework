using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TgBotFramework;

namespace EchoBotProject.EchoHandlers
{
    public class DefaultEcho : IUpdateHandler<BotExampleContext>
    {
        private readonly ILogger<DefaultEcho> _logger;

        public DefaultEcho(ILogger<DefaultEcho> logger)
        {
            _logger = logger;
        }
        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update {0}, generic handler for {1}", context.Update.Id, context.Update.Type);
        }
    }
}