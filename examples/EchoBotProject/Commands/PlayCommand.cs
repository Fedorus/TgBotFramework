using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgBotFramework;
using TgBotFramework.Attributes;
using TgBotFramework.WrapperExtensions;

namespace EchoBotProject.Commands
{
    [Command("play")]
    public class PlayCommand : CommandBase<BotExampleContext>
    {
        public override async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> next, string[] args, CancellationToken cancellationToken)
        {
            context.UserState.Stage = "game";
            await context.Client.SendTextMessageAsync(context.Update.GetChat(),
                "Okay, lets play. You are in the room, find exit", cancellationToken: cancellationToken);
        }
    }
}