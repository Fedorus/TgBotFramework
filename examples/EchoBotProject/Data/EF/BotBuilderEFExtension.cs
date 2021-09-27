using EchoBotProject.StateMachineBoilerplate;
using Microsoft.Extensions.DependencyInjection;
using TgBotFramework;

namespace EchoBotProject.Data.EF
{
    public static class BotBuilderEfExtension
    {
        public static IBotFrameworkBuilder<TContext> UseEF<TContext>(this IBotFrameworkBuilder<TContext> builder) where TContext : IUpdateContext
        {
            builder.Services.AddScoped<UserStateMapperMiddleware<TContext>>();
            builder.Services.AddScoped<UserStageManager>();
            builder.UpdatePipelineSettings.Middlewares.Add(typeof(UserStateMapperMiddleware<TContext>));
            return builder;
        }
    }
    
}