using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;

namespace EchoBotProject.Data.MongoDB
{
    public class UserStateMapper<TContext> : IUpdateHandler<TContext> where TContext : IUpdateContext
    {
        private readonly UserStageManager _manager;
        private readonly IMongoCollection<UserModel> _db;
        public UserStateMapper(IMongoDatabase db, UserStageManager manager)
        {
            _manager = manager;
            _db = db.GetCollection<UserModel>("Framework.UserModel");
        }

        public async Task HandleAsync(TContext context, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            var userId = context.Update.GetSenderId();
            context.UserState = _manager;
            UserModel userObj = null;
            if (userId != 0)
            {
                userObj = (await _db.FindAsync(x => x.Id == userId, cancellationToken: cancellationToken)).FirstOrDefault();
                if (userObj == null)
                {
                    userObj = new UserModel(){Id = userId};
                    await _db.InsertOneAsync(userObj, cancellationToken: cancellationToken);
                }
                else
                {
                    UserModelMapper.MapModelToState(context.UserState, userObj);
                }
            }
    
            await next(context, cancellationToken);

            if (userObj != null && UserModelMapper.MapStateToModel(context.UserState, userObj))
            {
                await _db.ReplaceOneAsync(x => x.Id == userId, userObj, cancellationToken: cancellationToken);
            }
        }
    }
}