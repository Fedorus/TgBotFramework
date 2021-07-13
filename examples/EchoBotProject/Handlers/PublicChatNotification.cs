using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;

namespace EchoBotProject.Handlers
{
    public class PublicChatNotification : IUpdateHandler<BotExampleContext>
    {
        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            await context.Client.SendTextMessageAsync(context.Update.GetChat(), "This update was from public chat", cancellationToken: cancellationToken);
        }
    }
}