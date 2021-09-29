using System.Threading;
using System.Threading.Tasks;
using EchoBotProject.StateMachineBoilerplate;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;

namespace EchoBotProject.Data.EF
{
    public class UserStateMapperMiddleware<TContext> : IUpdateHandler<TContext> where TContext : IUpdateContext
    {
        private readonly BotFrameworkContext _context;
        private readonly UserStageManager _manager;

        public UserStateMapperMiddleware(BotFrameworkContext context, UserStageManager manager)
        {
            _context = context;
            _manager = manager;
        }

        public async Task HandleAsync(TContext context, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            var userId = context.Update.GetSenderId();
            context.UserState = _manager;
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
            }
        }
    }
}