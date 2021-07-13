using System.Threading;
using System.Threading.Tasks;
using TgBotFramework;
using TgBotFramework.Attributes;
namespace EchoBotProject.States
{
    //[State(Stage = "input_something")]
    public class InputState<TContext> : BasicState, IUpdateHandler<TContext> where TContext : IUpdateContext 
    {
        public async Task HandleAsync(TContext context, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public InputState(TContext state) : base(state)
        {
        }
    }
}