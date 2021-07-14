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
    public class GameState<TContext> : BasicState, IUpdateHandler<TContext> where TContext : IUpdateContext 
    {
        public async Task HandleAsync(TContext context, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            if (context.Update.Message?.Text != null && context.Update.Message.Text == "/exit")
            {
                await this.Exit();
                await context.Client.SendTextMessageAsync(context.Update.GetChat(), "Okay, you got out, lets play ping-pong with counter", cancellationToken: cancellationToken);
            }
            else
            {
                await context.Client.SendTextMessageAsync(context.Update.GetChat(), "Cmon, find EXIT", cancellationToken: cancellationToken);
            }
        }

        public GameState(TContext state) : base(state)
        {
        }
    }
}