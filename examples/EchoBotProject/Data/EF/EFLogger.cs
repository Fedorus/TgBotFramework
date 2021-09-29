using System;
using System.Threading;
using System.Threading.Tasks;
using EchoBotProject.Data.EF.Models;
using EchoBotProject.Handlers;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;
using TgBotFramework;
using TgBotFramework.WrapperExtensions;
using ChatMemberStatus = EchoBotProject.Data.EF.Models.ChatMemberStatus;

namespace EchoBotProject.Data.EF
{
    public class EFLogger: IUpdateHandler<BotExampleContext>
    {
        private readonly BotFrameworkContext _db;

        public EFLogger(BotFrameworkContext db)
        {
            _db = db;
        }

        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            var update = context.Update;

            var user = update.GetSender();
            var chat = update.GetChat();

            Update updateObj = new Update();
            User userInDb = null;
            if (user != null)
            {
                userInDb = await _db.Users.FindAsync( user.Id ).ConfigureAwait(false);
                if (userInDb == null)
                {
                    userInDb = (await _db.Users.AddAsync(new User()
                    {
                        FirstName = user.FirstName, State = new State(), IsBot = user.IsBot, Username = user.Username,
                        LanguageCode = user.LanguageCode, LastName = user.LastName, UserId = user.Id,
                        PrivateChatStatus = chat?.Type == ChatType.Private
                            ? PrivateChatStatus.Active
                            : PrivateChatStatus.NotStarted
                    }, cancellationToken).ConfigureAwait(false)).Entity;
                }

                updateObj.User = userInDb;
            }

            GroupChat chatInDb = null;
            if (chat != null && (chat.Type == ChatType.Group || chat.Type == ChatType.Supergroup))
            {
                chatInDb = await _db.GroupChats.FindAsync( chat.Id );

                if (chatInDb is null )
                {
                    chatInDb = ( await _db.GroupChats.AddAsync(new GroupChat() { GroupChatId  = chat.Id, Name = chat.Title, ChatType = chat.Type}, cancellationToken)).Entity;
                    await _db.ChatMemberStatus.AddAsync(new ChatMemberStatus()
                        { User = userInDb, Status = Status.Member, GroupChat = chatInDb }, cancellationToken);
                }

                updateObj.GroupChat = chatInDb;
            }
            updateObj.Type = update.Type;
            updateObj.DateTime = DateTime.UtcNow;
            updateObj.UpdateObject = update.ToJsonString();

            await _db.Updates.AddAsync(updateObj, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            
            await next(context, cancellationToken);
        }
    }
}