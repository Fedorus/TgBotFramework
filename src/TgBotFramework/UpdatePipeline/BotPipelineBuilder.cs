using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TgBotFramework.DataStructures;

namespace TgBotFramework.UpdatePipeline
{
    public class BotPipelineBuilder<TContext> : IBotPipelineBuilder<TContext>
        where TContext : IUpdateContext
    {
        internal UpdateDelegate<TContext> UpdateDelegate { get; private set; }

        private readonly ICollection<Func<UpdateDelegate<TContext>, UpdateDelegate<TContext>>> _components;
        public ILogger<IBotPipelineBuilder<TContext>> Logger { get; }

        public BotPipelineBuilder(ILogger<IBotPipelineBuilder<TContext>> logger)
        {
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
            _components.Add(
                next =>
                    (context, cancellationToken) =>
                    {
                        if (context.Services.GetService(typeof(THandler)) is IUpdateHandler<TContext> handler)
                            return handler.HandleAsync(context, next, cancellationToken);
                        else
                            throw new NullReferenceException(
                                $"Unable to resolve handler of type {typeof(THandler).FullName}"
                            );
                    }
            );

            return this;
        }

        internal IBotPipelineBuilder<TContext> Use(Type type)
        {
            if (type.GetInterfaces().All(x => x != typeof(IUpdateHandler<TContext>)))
            {
                //TODO better type
                throw new Exception();
            }
            _components.Add(
                next =>
                    (context, cancellationToken) =>
                    {
                        if (context.Services.GetService(type) is IUpdateHandler<TContext> handler)
                            return handler.HandleAsync(context, next, cancellationToken);
                        else
                            throw new NullReferenceException(
                                $"Unable to resolve handler of type {type.FullName}"
                            );
                    }
            );

            return this;
        }

        internal IBotPipelineBuilder<TContext> CheckStates(SortedDictionary<string, Type> states)
        {
            _components.Add(next=> (context, cancellationToken) =>
            {
                //TODO replace with some other structure
                var type = states.PrefixSearch(context.UserState.Stage);
                if (type!=null && context.Services.GetService(type) is IUpdateHandler<TContext> handler)
                {
                    return handler.HandleAsync(context, next, cancellationToken);
                }
                else
                {
                    return next(context, cancellationToken);
                }
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

       
    }
}