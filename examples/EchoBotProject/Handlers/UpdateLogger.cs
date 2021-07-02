using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;

namespace EchoBotProject.Handlers
{
    public class UpdateLogger : IUpdateHandler<BaseUpdateContext>
    {
        private readonly ILogger<UpdateLogger> _logger;

        public UpdateLogger(ILogger<UpdateLogger> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(BaseUpdateContext context, UpdateDelegate<BaseUpdateContext> next, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update {0}, contents:\n{1}", context.Update.Id, context.Update.ToJsonString() );
            await next(context, cancellationToken);
        }
    }
}