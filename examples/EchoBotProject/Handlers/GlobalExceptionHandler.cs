using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TgBotFramework;

namespace EchoBotProject.Handlers
{
    public class GlobalExceptionHandler : IUpdateHandler<BaseUpdateContext>
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(BaseUpdateContext context, UpdateDelegate<BaseUpdateContext> next, CancellationToken cancellationToken)
        {
            try
            {
                await next(context, cancellationToken);
                _logger.LogInformation("Update {0}, no errors", context.Update.Id);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Update {0}, has errors {1}", context.Update.Id, e);
            }
        }
    }
}