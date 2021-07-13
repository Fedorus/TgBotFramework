using System;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TgBotFramework.Data.EF
{
    public static class BotBuilderEFExtension
    {
        public static IBotFrameworkBuilder<TContext> UseEF<TContext>(this IBotFrameworkBuilder<TContext> builder) where TContext : IUpdateContext
        {
            builder.Services.AddScoped<UserStateMapper<TContext>>();
            builder.UpdatePipelineSettings.Middlewares.Add(typeof(UserStateMapper<TContext>));
            return builder;
        }
    }
    
}