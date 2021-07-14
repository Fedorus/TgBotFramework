using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;

namespace EchoBotProject.Commands
{
    public class StartCommand : CommandBase<BotExampleContext>
    {
        public override async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> next, string[] args, CancellationToken cancellationToken)
        {
            await context.Client.SendTextMessageAsync(context.Update.GetChat(), "You`ve hit start!", cancellationToken: cancellationToken);
        }
    }
}