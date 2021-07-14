using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using TgBotFramework.DataStructures;
using TgBotFramework.Exceptions;

namespace TgBotFramework.UpdatePipeline
{
    public class BotPipelineBuilder<TContext> : IBotPipelineBuilder<TContext>
        where TContext : IUpdateContext
    {
        public ServiceCollection ServiceCollection { get; }
        internal UpdateDelegate<TContext> UpdateDelegate { get; private set; }

        private readonly ICollection<Func<UpdateDelegate<TContext>, UpdateDelegate<TContext>>> _components;
        public ILogger<IBotPipelineBuilder<TContext>> Logger { get; }

        public BotPipelineBuilder(ILogger<IBotPipelineBuilder<TContext>> logger, ServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
            Logger = logger;
            _components = new List<Func<UpdateDelegate<TContext>, UpdateDelegate<TContext>>>();
        }

        public UpdateDelegate<TContext> Build()
        {
            UpdateDelegate<TContext> handle = (context, cancellationToken) =>
            {
                Logger.LogWarning("No handler for update {0} of type {1}.", context.Update.Id, context.Update.Type);
                return Task.FromResult(1);
            };

            foreach (var component in _components.Reverse())
            {
                handle = component(handle);
            }

            return UpdateDelegate = handle;
        }

        public IBotPipelineBuilder<TContext> Use(Func<UpdateDelegate<TContext>, UpdateDelegate<TContext>> middleware)
        {
            _components.Add(middleware);
            return this;
        }

        public IBotPipelineBuilder<TContext> Use<THandler>()
            where THandler : IUpdateHandler<TContext>
        {
            ServiceCollection.TryAddScoped(typeof(THandler));
            _components.Add(
                next =>
                    (context, cancellationToken) =>
                    {
                        if (context.Services.GetService(typeof(THandler)) is IUpdateHandler<TContext> handler)
                            return handler.HandleAsync(context, next, cancellationToken);
                        else
                            throw new PipelineException(
                                $"Unable to resolve handler of type {typeof(THandler).FullName}"
                            );
                    }
            );

            return this;
        }

        internal IBotPipelineBuilder<TContext> Use(Type type)
        {
            ServiceCollection.TryAddScoped(type);
            if (type.GetInterfaces().All(x => x != typeof(IUpdateHandler<TContext>)))
            {
                //TODO better type
                throw new PipelineException($"Type {type} doesn't implement {typeof(IUpdateHandler<TContext>)}");
            }

            _components.Add(
                next =>
                    (context, cancellationToken) =>
                    {
                        if (context.Services.GetService(type) is IUpdateHandler<TContext> handler)
                            return handler.HandleAsync(context, next, cancellationToken);
                        else
                            throw new PipelineException(
                                $"Unable to resolve handler of type {type.FullName}"
                            );
                    }
            );

            return this;
        }

        internal IBotPipelineBuilder<TContext> CheckStages(SortedDictionary<string, Type> stages)
        {
            foreach (KeyValuePair<string,Type> pair in stages)
            {
                ServiceCollection.AddScoped(pair.Value);
            }
            _components.Add(next => (context, cancellationToken) =>
            {
                var type = stages.PrefixSearch(context.UserState.Stage);
                if (type != null)
                {
                    var realType = type;
                    if (type.IsGenericTypeDefinition)
                    {
                        realType = type.MakeGenericType(typeof(TContext));
                    }
                    if(context.Services.GetService(realType) is IUpdateHandler<TContext> handler)
                        return handler.HandleAsync(context, next, cancellationToken);
                    else
                    {
                        throw new PipelineException("Class wasn't registered: {0}", realType.FullName);
                    }
                } 
                return next(context, cancellationToken);
            });

            return this;
        }
        internal IBotPipelineBuilder<TContext> CheckCommands(SortedDictionary<string, Type> commands)
        {
            foreach (KeyValuePair<string,Type> pair in commands)
            {
                ServiceCollection.AddScoped(pair.Value);
            }
            _components.Add(next => (context, cancellationToken) =>
            {
                if (string.IsNullOrWhiteSpace(context.Update.Message?.Text) || !(context.Update.Message.Text.StartsWith('/') || context.Update.Message.Text.Length>1 ) )
                {
                    return next(context, cancellationToken); 
                }
                
                var type = commands.PrefixSearch(context.Update.Message.Text[1..]);
                if (type != null)
                {
                    var realType = type;
                    if (type.IsGenericTypeDefinition)
                    {
                        realType = type.MakeGenericType(typeof(TContext));
                    }

                    if (context.Services.GetService(realType) is IUpdateHandler<TContext> handler)
                        return handler.HandleAsync(context, next, cancellationToken);
                    else
                    {
                        throw new PipelineException("Class wasn't registered: {0}", realType.FullName);
                    }
                }

                return next(context, cancellationToken);

            });

            return this;
        }
        

        public IBotPipelineBuilder<TContext> Use<THandler>(THandler handler)
            where THandler : IUpdateHandler<TContext>
        {
            _components.Add(next =>
                (context, cancellationToken) => handler.HandleAsync(context, next, cancellationToken)
            );

            return this;
        }

        public IBotPipelineBuilder<TContext> UseWhen(
            Predicate<TContext> predicate,
            Action<IBotPipelineBuilder<TContext>> configure)
        {
            var branchBuilder = new BotPipelineBuilder<TContext>(this.Logger, ServiceCollection);
            configure(branchBuilder);
            UpdateDelegate<TContext> branchDelegate = branchBuilder.Build();

            Use(new UseWhenMiddleware<TContext>(predicate, branchDelegate));

            return this;
        }


        public IBotPipelineBuilder<TContext> UseWhen<THandler>(
            Predicate<TContext> predicate
        )
            where THandler : IUpdateHandler<TContext>
        {
            ServiceCollection.TryAddScoped(typeof(THandler));
            var branchDelegate = new BotPipelineBuilder<TContext>(Logger, ServiceCollection).Use<THandler>().Build();
            Use(new UseWhenMiddleware<TContext>(predicate, branchDelegate));

            return this;
        }

        public IBotPipelineBuilder<TContext> MapWhen(
            Predicate<TContext> predicate,
            Action<IBotPipelineBuilder<TContext>> configure)
        {
            var mapBuilder = new BotPipelineBuilder<TContext>(Logger, ServiceCollection);
            configure(mapBuilder);
            var mapDelegate = mapBuilder.Build();

            Use(new MapWhenMiddleware<TContext>(predicate, mapDelegate));

            return this;
        }

        public IBotPipelineBuilder<TContext> MapWhen<THandler>(
            Predicate<TContext> predicate)
            where THandler : IUpdateHandler<TContext>
        {
            ServiceCollection.TryAddScoped(typeof(THandler));
            var branchDelegate = new BotPipelineBuilder<TContext>(Logger, ServiceCollection).Use<THandler>().Build();

            Use(new MapWhenMiddleware<TContext>(predicate, branchDelegate));

            return this;
        }

        public IBotPipelineBuilder<TContext> UseCommand<TCommand>(
            string command
        )
            where TCommand : CommandBase<TContext>
        {
           return MapWhen(
                    ctx => ctx.Bot.CanHandleCommand(command, ctx.Update.Message),
                    botBuilder => botBuilder.Use<TCommand>()
                );
        }
    }
}
