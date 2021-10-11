using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TgBotFramework.Data.EF.Models;
using TgBotFramework.WrapperExtensions;
using ChatMemberStatus = TgBotFramework.Data.EF.Models.ChatMemberStatus;
using Update = TgBotFramework.Data.EF.Models.Update;
using User = TgBotFramework.Data.EF.Models.User;

namespace TgBotFramework.Data.EF
{
    public class PerUserStrategy<TContext> : IUpdateHandler<TContext> where TContext : IUpdateContext
    {
        private readonly BotFrameworkContext _db;

        public PerUserStrategy(BotFrameworkContext db)
        {
            _db = db;
        }

        public async Task HandleAsync(TContext context, UpdateDelegate<TContext> next,
            CancellationToken cancellationToken)
        {
            var update = context.Update;

            var user = update.GetSender();
            var chat = update.GetChat();

            Update updateObj = new Update();
            updateObj.User = await NewMethod(cancellationToken, user, chat, updateObj);

            GroupChat chatInDb = null;
            await NewMethod1(cancellationToken, chat, updateObj.User, updateObj);

            updateObj.Type = update.Type;
            updateObj.DateTime = DateTime.UtcNow;
            try
            {
                updateObj.UpdateObject = update.ToJsonString();
            }
            catch
            {
                //TODO fix Telegram.Bot until_date field serialization 
            }


            await _db.Updates.AddAsync(updateObj, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            await next(context, cancellationToken);
        }

        private async Task NewMethod1(CancellationToken cancellationToken, Chat chat, User userInDb, Update updateObj)
        {
            GroupChat chatInDb;
            if (chat != null && (chat.Type == ChatType.Group || chat.Type == ChatType.Supergroup))
            {
                chatInDb = await _db.GroupChats.FindAsync(chat.Id);

                if (chatInDb is null)
                {
                    chatInDb = (await _db.GroupChats.AddAsync(
                        new GroupChat() { GroupChatId = chat.Id, Name = chat.Title, ChatType = chat.Type },
                        cancellationToken)).Entity;
                    await _db.ChatMemberStatus.AddAsync(new ChatMemberStatus()
                        { User = userInDb, Status = Status.Member, GroupChat = chatInDb }, cancellationToken);
                }

                updateObj.GroupChat = chatInDb;
            }
        }

        private async Task<User> NewMethod(CancellationToken cancellationToken, Telegram.Bot.Types.User user, Chat chat, Update updateObj)
        {
            User userInDb = null;
            if (user != null)
            {
                userInDb = await _db.Users.FindAsync(user.Id).ConfigureAwait(false);
                if (userInDb == null)
                {
                    userInDb = (await _db.Users.AddAsync(new User()
                    {
                        FirstName = user.FirstName, State = new State() { Language = user.LanguageCode },
                        IsBot = user.IsBot, Username = user.Username,
                        LanguageCode = user.LanguageCode, LastName = user.LastName, UserId = user.Id,
                        PrivateChatStatus = chat?.Type == ChatType.Private
                            ? PrivateChatStatus.Active
                            : PrivateChatStatus.NotStarted
                    }, cancellationToken).ConfigureAwait(false)).Entity;
                }
            }

            return userInDb;
        }
    }
}