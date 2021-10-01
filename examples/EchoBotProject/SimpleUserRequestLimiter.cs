using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;

namespace EchoBotProject
{
    public class SimpleUserRequestLimiter : IUpdateHandler<BotExampleContext>
    {
        private readonly ILogger<SimpleUserRequestLimiter> _logger;
        private static readonly Dictionary<long, Queue<DateTime>> _requests = new Dictionary<long, Queue<DateTime>>();
        const int RequestsPerSecond = 3;

        public SimpleUserRequestLimiter(ILogger<SimpleUserRequestLimiter> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            var userId = context.Update.GetSenderId();
            
            // 0 for requests without user
            if (userId != 0)
            {
                if (_requests.ContainsKey(userId))
                {
                    var item = _requests[userId];
                    if (item.Count == RequestsPerSecond)
                    {
                        if ((DateTime.Now - item.Peek()) < TimeSpan.FromSeconds(1))
                        {
                            _logger.LogWarning("Ignoring update {0}", context.Update.Id);
                            return;
                        }
                        else
                        {
                            item.Dequeue();
                            item.Enqueue(DateTime.Now);
                        }
                    }
                    else
                    {
                        item.Enqueue(DateTime.Now);
                    }
                }
                else
                {
                    var item = new Queue<DateTime>(RequestsPerSecond);
                    item.Enqueue(DateTime.Now);
                    _requests.Add(userId, item);
                }
            }

            await next(context, cancellationToken);
        }
    }
}