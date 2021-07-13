using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using TgBotFramework.Models;
using TgBotFramework.WrapperExtensions;

namespace TgBotFramework.Data.MongoDB
{
    public class UserStateMapper<TContext> : IUpdateHandler<TContext> where TContext : IUpdateContext
    {
        private readonly IMongoCollection<UserModel> _db;
        public UserStateMapper(IMongoDatabase db)
        {
            _db = db.GetCollection<UserModel>("Framework.UserModel");
        }

        public async Task HandleAsync(TContext context, UpdateDelegate<TContext> next, CancellationToken cancellationToken)
        {
            var userId = context.Update.GetSenderId();
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

            if (UserModelMapper.MapStateToModel(context.UserState, userObj))
            {
                await _db.ReplaceOneAsync(x => x.Id == userId, userObj, cancellationToken: cancellationToken);
            }
        }
    }
}