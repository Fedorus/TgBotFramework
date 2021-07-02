using System.Threading.Tasks;

namespace TgBotFramework
{
    public abstract class BasicState
    {
        public IUpdateContext State { get;  }

        public BasicState(IUpdateContext state)
        {
            State = state;
        }

        public virtual async Task Enter()
        {
            
        }
        
        public virtual async Task Exit()
        {
            State.UserState.Stage = "default";
            State.UserState.Step = 0;
        }
    }
}