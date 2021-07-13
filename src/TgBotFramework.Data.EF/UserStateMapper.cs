using System;
using System.Threading;
using System.Threading.Tasks;
using TgBotFramework.Models;
using TgBotFramework.WrapperExtensions;

namespace TgBotFramework.Data.EF
{
    public class UserStateMapper<TContext> : IUpdateHandler<TContext> where TContext : IUpdateContext
    {
        private readonly BotFrameworkContext _context;
        public UserStateMapper(BotFrameworkContext context)
        {
            _context = context;
        }

        public async Task HandleAsync(TContext context, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            var userId = context.Update.GetSenderId();

            var userDbObject = await _context.UserModels.FindAsync(userId);
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

            await next(context, cancellationToken); 

            if (UserModelMapper.MapStateToModel(context.UserState, userDbObject))
            {
                _context.UserModels.Update(userDbObject);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

    }
}