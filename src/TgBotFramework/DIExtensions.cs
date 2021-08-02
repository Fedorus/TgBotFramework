using System;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgBotFramework.UpdatePipeline;

namespace TgBotFramework
{
    // ReSharper disable once InconsistentNaming
    public static class DIExtensions
    {
        public static IServiceCollection AddBotService<TBot, TContext>
            (this IServiceCollection services, Action<IBotFrameworkBuilder<TContext>> configure) 
            where TBot : BaseBot
            where TContext : BaseUpdateContext 
        {
            var builder = new BotFrameworkBuilder<TContext, TBot>(services);
            configure(builder);

            return services;
        }
        
        
        public static void EnsureWebhookSet<TBot>(
            this IServiceProvider app
        )
            where TBot : BaseBot
        {
            using (var scope = app.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<TBot>>();
                var bot = scope.ServiceProvider.GetRequiredService<TBot>();
                var options = scope.ServiceProvider.GetRequiredService<IOptions<BotSettings>>();
                var url = new Uri(new Uri(options.Value.WebhookDomain), options.Value.WebhookPath);

                logger.LogInformation("Setting webhook for bot \"{0}\" to URL \"{1}\"", typeof(TBot).Name, url);

                bot.Client.SetWebhookAsync(url.AbsoluteUri)
                    .GetAwaiter().GetResult();
            }
        }
    }
}