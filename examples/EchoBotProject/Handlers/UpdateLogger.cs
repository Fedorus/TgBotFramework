using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;

namespace EchoBotProject.Handlers
{
    public class UpdateLogger : IUpdateHandler<BotExampleContext>
    {
        private readonly ILogger<UpdateLogger> _logger;

        public UpdateLogger(ILogger<UpdateLogger> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update {0}, contents:\n{1}", context.Update.Id, context.Update.ToJsonString() );
            await next(context, cancellationToken);
        }
    }
}