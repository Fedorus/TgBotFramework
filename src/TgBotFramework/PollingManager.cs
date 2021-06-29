using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using TgBotFramework.UpdatePipeline;

namespace TgBotFramework
{
    public class PollingManager<TContext> : BackgroundService, IPollingManager<TContext>
        where TContext : IUpdateContext
    {
        private readonly ILogger<PollingManager<TContext>> _logger;
        private readonly LongPollingOptions _pollingOptions;
        private readonly IServiceProvider _serviceProvider;
        private readonly ChannelWriter<IUpdateContext> _channel;
        private readonly TelegramBotClient Client;
        

        public PollingManager(
            ILogger<PollingManager<TContext>> logger, 
            LongPollingOptions pollingOptions, 
            IOptions<BotSettings> botOptions, 
            Channel<IUpdateContext> channel,
            IServiceProvider serviceProvider) 
        {
            _logger = logger;
            _pollingOptions = pollingOptions;
            _serviceProvider = serviceProvider;
            _channel = channel.Writer;
            Client = new TelegramBotClient(botOptions.Value.ApiToken, baseUrl: botOptions.Value.BaseUrl);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            int messageOffset = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var updates = await Client.GetUpdatesAsync(messageOffset, 0, _pollingOptions.Timeout,
                        _pollingOptions.AllowedUpdates, cancellationToken);

                    foreach (var update in updates)
                    {
                        var updateContext = _serviceProvider.GetService<IUpdateContext>();
                        
                        Debug.Assert(updateContext != null, nameof(updateContext) + " != null");
                        updateContext.Update = update;
                        
                        await _channel.WriteAsync(updateContext, cancellationToken);
                        messageOffset = update.Id + 1;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error while polling in " + nameof(PollingManager<TContext>));
                }
            }
        }
    }

    public interface IPollingManager<in TContext> 
        where TContext : IUpdateContext
    {
    }
}