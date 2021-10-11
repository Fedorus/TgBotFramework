using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TgBotFramework;

namespace CommonHandlers
{
    public class GlobalExceptionHandler<T> : IUpdateHandler<T> where T : IUpdateContext
    {
        private readonly ILogger<GlobalExceptionHandler<T>> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler<T>> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(T context, UpdateDelegate<T> next,
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