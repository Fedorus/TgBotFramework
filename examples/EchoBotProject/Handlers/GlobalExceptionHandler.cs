using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TgBotFramework;

namespace EchoBotProject.Handlers
{
    public class GlobalExceptionHandler : IUpdateHandler<BotExampleContext>
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> next,
            CancellationToken cancellationToken)
        {
            try
            {
                await next(context, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogInformation(e,"[{0}] has errors {1}", context.Update.Id, e);
            }
        }
    }
}