using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TgBotFramework.Attributes;
using TgBotFramework.UpdatePipeline;

namespace TgBotFramework
{
    public class BotFrameworkBuilder<TContext> : IBotFrameworkBuilder<TContext> where TContext : IUpdateContext
    {
        public IServiceCollection Services { get; }
        public IUpdateContext Context { get; set; }

        public UpdatePipelineSettings<TContext> UpdatePipelineSettings { get; set; } =
            new UpdatePipelineSettings<TContext>();

        public IBotFrameworkBuilder<TContext> UseLongPolling<T>(LongPollingOptions longPollingOptions) where T : BackgroundService, IPollingManager<TContext>
        {
            Services.AddHostedService<T>();
            Services.AddSingleton(longPollingOptions);
            Services.AddSingleton<IPollingManager<TContext>>(x=>x.GetService<T>());
            return this;
        }

        public IBotFrameworkBuilder<TContext> UseMiddleware<TMiddleware>() where TMiddleware : IUpdateHandler<TContext>
        {
            UpdatePipelineSettings.Middlewares.Add(typeof(TMiddleware));

            return this;
        }
        public IBotFrameworkBuilder<TContext> SetPipeline
        (
            Func<IBotPipelineBuilder<TContext>, IBotPipelineBuilder<TContext>> pipeBuilder) 
        {
            UpdatePipelineSettings.PipeSettings = pipeBuilder;
            
            return this;
        }

        public IBotFrameworkBuilder<TContext> UseStates(Assembly assembly)
        {
            var types = assembly.GetTypes().Where(x => 
                    x.GetCustomAttribute<StateAttribute>()!=null && 
                    x.BaseType == typeof(BasicState) && 
                    x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IUpdateHandler<>)
                ))
                .ToList();

            UpdatePipelineSettings.States.AddRange(types);
            
            return this;
        }

        public IBotFrameworkBuilder<TContext> UseCommands(Assembly assembly)
        {
            var types = assembly.GetTypes().Where(x => 
                    x.GetCustomAttribute<CommandAttribute>()!=null && 
                    x.BaseType == typeof(CommandBase<TContext>) && 
                    x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IUpdateHandler<>)
                    ))
                .ToList();

            UpdatePipelineSettings.Commands.AddRange(types);
            
            return this;
        }


        public BotFrameworkBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}