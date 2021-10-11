using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework.Data.EF.Models;
using TgBotFramework.WrapperExtensions;

namespace TgBotFramework.Data.EF
{
    public class UserStateMapperMiddleware<TContext> : IUpdateHandler<TContext> where TContext : IUpdateContext
    {
        private readonly BotFrameworkContext _db;
        private readonly UserStageManager _manager;

        public UserStateMapperMiddleware(BotFrameworkContext db, UserStageManager manager)
        {
            _db = db;
            _manager = manager;
        }

        public async Task HandleAsync(TContext context, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            var userId = context.Update.GetSenderId();
            var chat = context.Update.GetChat();
            context.UserState = _manager;

            State state = null;
            if (userId != 0)
            {
                state = _db.States.First(x => x.User.UserId == userId && (x.GroupChat == null));
            }

            if (state != null)
            {
                context.UserState.Role = state.Role;
                context.UserState.Stage = state.Stage;
                context.UserState.Step = state.Step;
                context.UserState.LanguageCode = state.User.LanguageCode;
            }

            await next(context, cancellationToken);
            if (state != null)
            {
                state.Role = context.UserState.Role;
                state.Stage = context.UserState.Stage;
                state.Step = context.UserState.Step;
                _db.States.Update(state);
                await  _db.SaveChangesAsync(cancellationToken);
            }

            /*
            UserModel userDbObject = null;
            if (userId != 0)
            {
                userDbObject = await _context.UserModels.FindAsync(userId);
                if (userDbObject == null)
                {
                    userDbObject = new UserModel();
                    userDbObject.Id = userId;
                    userDbObject.Stage = "default";
                    userDbObject.Step = 0;
                    await _context.UserModels.AddAsync(userDbObject, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                    userDbObject = await _context.UserModels.FindAsync(userId);
                }
                UserModelMapper.MapModelToState(context.UserState, userDbObject);
            }


            await next(context, cancellationToken); 

            if (userDbObject != null && UserModelMapper.MapStateToModel(context.UserState, userDbObject))
            {
                _context.UserModels.Update(userDbObject);
                await _context.SaveChangesAsync(cancellationToken);
            }*/
        }
    }
}