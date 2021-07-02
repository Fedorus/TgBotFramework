using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;

namespace EchoBotProject.Handlers
{
    public class MessageHandler : IUpdateHandler<BaseUpdateContext>
    {
        public async Task HandleAsync(BaseUpdateContext context, UpdateDelegate<BaseUpdateContext> next, CancellationToken cancellationToken)
        {
            if (context.Update.Type == UpdateType.Message)
            {
                await context.Client.SendTextMessageAsync(context.Update.GetChat().Id, "Pong", cancellationToken: cancellationToken);
            }
        }
    }
}