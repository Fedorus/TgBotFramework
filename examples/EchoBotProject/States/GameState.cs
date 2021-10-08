using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TgBotFramework;
using TgBotFramework.Attributes;
using TgBotFramework.WrapperExtensions;
using TgBotFramework.MessageReader;

namespace EchoBotProject.States
{
    [State(Stage = "game")]
    public class GameState<TContext> : BasicState<TContext> where TContext : IUpdateContext 
    {
        private readonly IMessageReader<GameState<TContext>, TContext> _messageReader;

        public GameState(IMessageReader<GameState<TContext>, TContext> messageReader)
        {
            _messageReader = messageReader;
        }

        public override async Task HandleAsync(TContext context, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            if (context.Update.Message?.Text != null && context.Update.Message.Text == "/exit")
            {
                await context.Client.SendTextMessageAsync(context.Update.GetChat(), _messageReader.GetMessage("exit"), cancellationToken: cancellationToken);
                await Exit(context);
            }
            else
            {
                await context.Client.SendTextMessageAsync(context.Update.GetChat(), _messageReader.GetMessage("exit.provocativeMessage"), cancellationToken: cancellationToken);
            }
        }
    }
}