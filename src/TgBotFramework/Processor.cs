using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgBotFramework.Attributes;
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
        private readonly TBot _bot;
        private readonly ChannelReader<IUpdateContext> _updatesQueue;
        private readonly UpdateDelegate<TContext> _updateHandler;
        private readonly SortedDictionary<string, Type> states;
        
        public BotService(ILogger<BotService<TBot, TContext>> logger,
            IServiceProvider serviceProvider,
            Channel<IUpdateContext> updatesQueue, 
            TBot bot, 
            UpdatePipelineSettings<TContext> updatePipelineSettings
            ) 
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _bot = bot;
            _updatesQueue = updatesQueue.Reader;

            var pipe = new BotPipelineBuilder<TContext>(_serviceProvider
                .GetService<ILogger<BotPipelineBuilder<TContext>>>());

            // add middlewares 
            foreach (var middleware in updatePipelineSettings.Middlewares)
            {
                if (middleware.GetInterfaces().Any(x=> x == typeof(IUpdateHandler<TContext>)))
                {
                    pipe.Use(middleware);
                }
            }
            
            // add states
            states = new SortedDictionary<string, Type>(StringComparer.Ordinal);
            foreach (var state in updatePipelineSettings.States)
            {
                var attribute = state.GetCustomAttribute<StateAttribute>();
                states.Add(attribute.Stage, state);
            }

            //pipe.CheckStates(states);
            
            //TODO Commands
            
            // adding pipeline
            updatePipelineSettings.PipeSettings(pipe);

            _updateHandler = pipe.Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var update in _updatesQueue.ReadAllAsync(stoppingToken))
            {
                using var scope = _serviceProvider.CreateScope();
                update.Services = scope.ServiceProvider;
                update.Client = _bot.Client;
                await _updateHandler((TContext)update, stoppingToken);
                
                if (update.Result !=null)
                {
                    //TODO: callback on finish
                    //Task.Run(() => update.Result.TrySetResult());
                }
            }
        }
    }
}