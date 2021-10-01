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
        /// <summary>
        /// Adds and configures telegram bot updates processing 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TBot"></typeparam>
        /// <typeparam name="TContext">Your context object</typeparam>
        /// <returns></returns>
        public static IServiceCollection AddBotService<TBot, TContext>
            (this IServiceCollection services, Action<IBotFrameworkBuilder<TContext>> configure) 
            where TBot : BaseBot
            where TContext : BaseUpdateContext 
        {
            var builder = new BotFrameworkBuilder<TContext, TBot>(services);
            configure(builder);

            return services;
        }
    }
}