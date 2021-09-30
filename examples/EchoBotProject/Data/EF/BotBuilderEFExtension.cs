using System;
using EchoBotProject.StateMachineBoilerplate;
using Microsoft.Extensions.DependencyInjection;
using TgBotFramework;

namespace EchoBotProject.Data.EF
{
    public static class BotBuilderEfExtension
    {
        public static IBotFrameworkBuilder<TContext> UseEF<TContext>(this IBotFrameworkBuilder<TContext> builder, StateStrategy strategy) where TContext : IUpdateContext
        {
            builder.Services.AddScoped<UserStageManager>();
            if (strategy == StateStrategy.PerUser)
            {
                builder.Services.AddScoped<UserStateMapperMiddleware<TContext>>();
                builder.Services.AddScoped<EFLogger>();
                builder.UpdatePipelineSettings.Middlewares.Add(typeof(EFLogger));
                builder.UpdatePipelineSettings.Middlewares.Add(typeof(UserStateMapperMiddleware<TContext>));
            }
            else
                throw new NotImplementedException("This strategy wasn`t implemented");

            return builder;
        }
    }
}