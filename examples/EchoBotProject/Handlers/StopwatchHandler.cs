using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TgBotFramework;

namespace EchoBotProject.Handlers
{
    public class StopwatchHandler : IUpdateHandler<BotExampleContext>
    {
        private readonly ILogger<StopwatchHandler> _logger;

        public StopwatchHandler(ILogger<StopwatchHandler> logger)
        {
            _logger = logger;
        }
        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            var sw = new Stopwatch();
            sw.Start();
            await next(context, cancellationToken);
            sw.Stop();
            _logger.LogInformation("Update {0} processed in {1}", context.Update.Id, sw.Elapsed);
        }
    }
}