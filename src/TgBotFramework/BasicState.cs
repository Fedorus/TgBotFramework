using System.Threading;
using System.Threading.Tasks;

namespace TgBotFramework
{
    public abstract class BasicState<TContext> : IUpdateHandler<TContext> where TContext : IUpdateContext 
    {
        public virtual async Task Enter(TContext state)
        {
            
        }
        
        public virtual async Task Exit(TContext state)
        {
            state.UserState.Stage = "default";
            state.UserState.Step = 0;
        }

        public abstract Task HandleAsync(TContext context, UpdateDelegate<TContext> next,
            CancellationToken cancellationToken);

    }
}