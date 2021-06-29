using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using TgBotFramework.UpdatePipeline;
using TgBotFramework.WrapperExtensions;

namespace TgBotFramework
{
    public class BotService<TBot, TContext> : BackgroundService  
        where TBot : BaseBot
        where TContext : IUpdateContext 
    {
        private readonly ILogger<BotService<TBot, TContext>> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ChannelReader<IUpdateContext> _updatesQueue;
        private readonly UpdateDelegate<TContext> _updateHandler;
        
        
        public BotService(ILogger<BotService<TBot, TContext>> logger,
            IServiceProvider serviceProvider,
            Channel<IUpdateContext> updatesQueue, 
            TBot bot) //, BotPipelineBuilder<TContext> pipelineSettings
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _updatesQueue = updatesQueue.Reader;
            
            //_updateHandler = pipelineSettings.Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var update in _updatesQueue.ReadAllAsync(stoppingToken))
            {
                _logger.LogInformation(update.Update.ToJsonString());
                //await _updateHandler((TContext)update, stoppingToken).ConfigureAwait(false);
                
                
                if (update.Result !=null)
                {
                    Task.Run(() => update.Result.TrySetResult());
                }
            }
        }
    }
}