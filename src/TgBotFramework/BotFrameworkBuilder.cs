using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TgBotFramework
{
    public class BotFrameworkBuilder<TContext> : IBotFrameworkBuilder<TContext> where TContext : IUpdateContext
    {
        public IServiceCollection Services { get; }
        public IUpdateContext Context { get; set; }
        public IBotFrameworkBuilder<TContext> UseLongPolling<T>(LongPollingOptions longPollingOptions) where T : BackgroundService, IPollingManager<TContext>
        {
            Services.AddHostedService<T>();
            Services.AddSingleton(longPollingOptions);
            Services.AddSingleton<IPollingManager<TContext>>(x=>x.GetService<T>());
            return this;
        }

        public BotFrameworkBuilder(IServiceCollection services)
        {
            Services = services;
        }

    }
}