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
using TgBotFramework.Exceptions;
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
        private SortedDictionary<string, Type> _stages;
        private SortedDictionary<string,Type> _commands;

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
                .GetService<ILogger<BotPipelineBuilder<TContext>>>(), new ServiceCollection());

            // add middlewares 
            foreach (var middleware in updatePipelineSettings.Middlewares)
            {
                if (middleware.GetInterfaces().Any(x=> x == typeof(IUpdateHandler<TContext>)))
                {
                    pipe.Use(middleware);
                }
                // check for other
            }
            
            // add states
            SetUpStagesInPipeline(updatePipelineSettings, pipe);

            // add commands
            SetUpCommandsInPipeline(updatePipelineSettings, pipe);

            // adding pipeline
            updatePipelineSettings.PipeSettings(pipe);

            CheckPipeline(pipe, serviceProvider);
            
            _updateHandler = pipe.Build();
        }

        private void CheckPipeline(BotPipelineBuilder<TContext> pipe, IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            foreach (var type in pipe.ServiceCollection)
            {
                if (scope.ServiceProvider.GetService(type.ImplementationType) == null)
                {
                    _logger.LogCritical("There is no service type of {0} in DI", type.ImplementationType.FullName);
                    throw new PipelineException(
                        string.Format("There is no service type of {0} in DI", type.ImplementationType.FullName));
                }
            }
        }

        private void SetUpStagesInPipeline(UpdatePipelineSettings<TContext> updatePipelineSettings, BotPipelineBuilder<TContext> pipe)
        {
            _stages = new SortedDictionary<string, Type>(StringComparer.Ordinal);
            foreach (var state in updatePipelineSettings.States)
            {
                var attribute = state.GetCustomAttribute<StateAttribute>();
                Debug.Assert(attribute != null, nameof(attribute) + " != null");
                _stages.Add(attribute.Stage, state);
            }
            // check for other
            if (_stages.Count != 0)
                pipe.CheckStages(_stages);
        }

        private void SetUpCommandsInPipeline(UpdatePipelineSettings<TContext> updatePipelineSettings, BotPipelineBuilder<TContext> pipe)
        {
            _commands = new SortedDictionary<string, Type>(StringComparer.Ordinal);
            foreach (var command in updatePipelineSettings.Commands)
            {
                var attribute = command.GetCustomAttribute<CommandAttribute>();
                Debug.Assert(attribute != null, nameof(attribute) + " != null");
                _commands.Add(attribute.Text, command);
            }
            // check for other
            if (_commands.Count != 0)
                pipe.CheckCommands(_commands);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            await foreach (var update in _updatesQueue.ReadAllAsync(stoppingToken))
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    update.Services = scope.ServiceProvider;
                    update.Client = _bot.Client;
                    update.Bot = _bot;
                    await _updateHandler((TContext) update, stoppingToken);

                    if (update.Result != null)
                    {
                        //TODO: callback on finish
                        //Task.Run(() => update.Result.TrySetResult());
                    }
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, "Oops");
                    
                }
            }
        }
    }
}