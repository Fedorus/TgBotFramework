using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TgBotFramework;
using TgBotFramework.Attributes;
using TgBotFramework.WrapperExtensions;

namespace EchoBotProject.States
{
    [State(Stage = "game")]
    public class GameState<TContext> : BasicState<TContext> where TContext : IUpdateContext 
    {
        public override async Task HandleAsync(TContext context, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            if (context.Update.Message?.Text != null && context.Update.Message.Text == "/exit")
            {
                await context.Client.SendTextMessageAsync(context.Update.GetChat(), "Okay, you got out, lets play ping-pong with counter", cancellationToken: cancellationToken);
                await Exit(context);
            }
            else
            {
                await context.Client.SendTextMessageAsync(context.Update.GetChat(), "Cmon, find EXIT", cancellationToken: cancellationToken);
            }
        }
    }
}