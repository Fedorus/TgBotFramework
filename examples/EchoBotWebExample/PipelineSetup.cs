using EchoBotProject;
using EchoBotProject.Commands;
using EchoBotProject.EchoHandlers;
using EchoBotProject.Handlers;
using EchoBotProject.States;
using Microsoft.Extensions.DependencyInjection;
using TgBotFramework;
using TgBotFramework.UpdatePipeline;

namespace EchoBotWebExample
{
    public static class PipelineSetup
    {
        /// <summary>
        /// Since Startup file may be huge and its hard to keep track whats added to DI it may be better to extract it to such method
        /// </summary>
        /// <returns></returns>
        public static IServiceCollection ServicesForExamplePipelineBuilder(this IServiceCollection services)
        {
            services.AddScoped<MessageHandler>();
            services.AddScoped<StartCommand>();
            services.AddScoped<ReturnState>();
            services.AddScoped<PublicChatEcho>();
            services.AddScoped<GameState<BotExampleContext>>();
            services.AddScoped<PlayCommand>();
            
            services.AddScoped<BotWasBlockedUnblockedOrAdded>();
            services.AddScoped<GroupChatUserUpdated>();
            services.AddScoped<DefaultEcho>();
            services.AddScoped<MemberAddedEcho>();
            return services;
        }

        /// <summary>
        /// Same with pipeline ending
        /// </summary>
        /// <returns></returns>
        public static IBotPipelineBuilder<BotExampleContext> ExamplePipelineBuilder
            (this IBotPipelineBuilder<BotExampleContext> pipe)
            => pipe
                .MapWhen(On.Message, onMessageBuilder => onMessageBuilder
                        .MapWhen(In.PrivateChat, branch => branch
                            .UseCommand<StartCommand>("start")
                            .Use<MessageHandler>()
                        )
                        .MapWhen(In.GroupOrSupergroup, groupPipe => groupPipe
                            // works when someone added or enters to chat
                            .MapWhen<MemberAddedEcho>(context => context.Update.Message.NewChatMembers != null)
                            // will be called if no MapWhen handler used
                            //don`t forget that you need privacy mode off to receive all messages from group (BotFather settings)
                            .Use<PublicChatEcho>()
                        )
                        // .MapWhen<PublicChatEcho>(In.GroupOrSupergroup)
                        // this was same as line below, just demonstration how you can combine such statements with your own
                        // .MapWhen<PublicChatEcho>(context => In.GroupChat(context) || In.SupergroupChat(context))
                )
                .MapWhen<DefaultEcho>(On.Poll)
                .MapWhen<DefaultEcho>(On.Unknown)
                .MapWhen<DefaultEcho>(On.CallbackQuery)
                .MapWhen<DefaultEcho>(On.ChannelPost)
                .MapWhen<DefaultEcho>(On.InlineQuery)
                .MapWhen<DefaultEcho>(On.EditedMessage)
                .MapWhen<DefaultEcho>(On.PollAnswer)
                .MapWhen<DefaultEcho>(On.ShippingQuery)
                .MapWhen<DefaultEcho>(On.ChosenInlineResult)
                .MapWhen<DefaultEcho>(On.EditedChannelPost)
                .MapWhen<DefaultEcho>(On.PreCheckoutQuery)
                // fired when user blocks/unblocks bot, adds user to chat
                .MapWhen<BotWasBlockedUnblockedOrAdded>(On.MyChatMember)
                .MapWhen<GroupChatUserUpdated>(On.ChatMember);
    }
}