using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;

namespace EchoBotProject.Handlers
{
    public class MessageHandler : IUpdateHandler<BotExampleContext>
    {
        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            if (context.Update.Type == UpdateType.Message)
            {
                await context.Client.SendTextMessageAsync(context.Update.GetChat().Id, "Pong "+context.UserState.Step, cancellationToken: cancellationToken);
                context.UserState.Step++;
            }
        }
    }
}