using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TgBotFramework;

namespace EchoBotProject.EchoHandlers
{
    public class GroupChatUserUpdated : IUpdateHandler<BotExampleContext>
    {
        private readonly ILogger<GroupChatUserUpdated> _logger;

        public GroupChatUserUpdated(ILogger<GroupChatUserUpdated> logger)
        {
            _logger = logger;
        }
        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update {0}, users {1} state changed {2} -> {3} in chat {4} by {5}", context.Update.Id,
                context.Update.ChatMember.NewChatMember.User, context.Update.ChatMember.OldChatMember.Status,
                context.Update.ChatMember.NewChatMember.Status, context.Update.ChatMember.Chat.Id, context.Update.ChatMember.From );
        }
    }
}