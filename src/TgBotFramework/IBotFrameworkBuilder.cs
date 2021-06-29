using System;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TgBotFramework.UpdatePipeline;

namespace TgBotFramework
{
    public interface IBotFrameworkBuilder<out TContext> where TContext : IUpdateContext
    {
        public IServiceCollection Services { get; }
        
        public IUpdateContext Context { get; set; }

        IBotFrameworkBuilder<TContext> UseLongPolling<T>(
            LongPollingOptions longPollingOptions)
            where T : BackgroundService, IPollingManager<TContext>;

    }
    
    public static class BotFrameworkBuilderExtensions
    {
        public static IBotFrameworkBuilder<TContext> SetPipeline<TContext>(
            this IBotFrameworkBuilder<TContext> builder, 
            Func<IBotPipelineBuilder<TContext>, IBotPipelineBuilder<TContext>> pipeBuilder
        ) where TContext : IUpdateContext
        {
            var pipe = new BotPipelineSettings<TContext>();
            pipe.PipeSettings = pipeBuilder;

            builder.Services.AddSingleton(pipe);
            return builder;
        }
    }

    public class BotPipelineSettings<TContext> where TContext : IUpdateContext
    {
        public Func<IBotPipelineBuilder<TContext>, IBotPipelineBuilder<TContext>> PipeSettings { get; set; }
    }
}