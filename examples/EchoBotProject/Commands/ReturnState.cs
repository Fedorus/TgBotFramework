using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TgBotFramework;
using TgBotFramework.Attributes;
using TgBotFramework.WrapperExtensions;

namespace EchoBotProject.Commands
{
    [Command("/state")]
    public class ReturnState : CommandBase<BaseUpdateContext>
    {
        public override async Task HandleAsync(BaseUpdateContext context, UpdateDelegate<BaseUpdateContext> next, string[] args, CancellationToken cancellationToken)
        {
            var stateText = $"Your role: {context.UserState.Role}\n" +
                            $"Your stage: {context.UserState.Stage}\n" +
                            $"Your step: {context.UserState.Step}";
            await context.Client.SendTextMessageAsync(context.Update.GetChat(), stateText, cancellationToken: cancellationToken);
        }
    }
}