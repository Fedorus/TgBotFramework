using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TgBotFramework.UpdatePipeline;

namespace TgBotFramework
{
    public interface IBotFrameworkBuilder<TContext> where TContext : IUpdateContext
    {
        IServiceCollection Services { get; }
        IUpdateContext Context { get; set; }
        UpdatePipelineSettings<TContext> UpdatePipelineSettings { get; set; }
        
        IBotFrameworkBuilder<TContext> UseLongPolling<T>(
            LongPollingOptions longPollingOptions)
            where T : BackgroundService, IPollingManager<TContext>;

        IBotFrameworkBuilder<TContext> UseMiddleware<TMiddleware>() where TMiddleware : IUpdateHandler<TContext>;

        IBotFrameworkBuilder<TContext> SetPipeline(
            Func<IBotPipelineBuilder<TContext>, IBotPipelineBuilder<TContext>> pipeBuilder);

        IBotFrameworkBuilder<TContext> UseStates();
        IBotFrameworkBuilder<TContext> UseStates(Assembly assembly);
        IBotFrameworkBuilder<TContext> UseCommands();
        IBotFrameworkBuilder<TContext> UseCommands(Assembly getAssembly);
    }
}