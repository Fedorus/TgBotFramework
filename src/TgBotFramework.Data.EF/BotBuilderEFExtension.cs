using System;
using Microsoft.Extensions.DependencyInjection;

namespace TgBotFramework.Data.EF
{
    public static class BotBuilderEfExtension
    {
        public static IBotFrameworkBuilder<TContext> UseEF<TContext, TDbContext>(this IBotFrameworkBuilder<TContext> builder, StateStrategy strategy) 
            where TContext : IUpdateContext
            where TDbContext : BotFrameworkContext
        {
            builder.Services.AddScoped<UserStageManager>();
            if (strategy == StateStrategy.PerUser)
            {
                builder.Services.AddScoped<UserStateMapperMiddleware<TContext>>();
                builder.Services.AddScoped<EFLogger<TContext>>();
                builder.UpdatePipelineSettings.Middlewares.Add(typeof(EFLogger<TContext>));
                builder.UpdatePipelineSettings.Middlewares.Add(typeof(UserStateMapperMiddleware<TContext>));
            }
            else
                throw new NotImplementedException("This strategy wasn't implemented");

            return builder;
        }

        public static IBotFrameworkBuilder<TContext> FullFunctionality<TContext, TDbContext>(
            this IBotFrameworkBuilder<TContext> builder)
            where TContext : IUpdateContext
            where TDbContext : BotFrameworkContext
        {
            return builder;
        }
    }
}