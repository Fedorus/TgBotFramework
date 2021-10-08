using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using TgBotFramework;
using TgBotFramework.Attributes;
using TgBotFramework.MessageReader;
using TgBotFramework.WrapperExtensions;

namespace EchoBotProject.Handlers
{
    [Handler("publicChat")]
    public class PublicChatEcho : IUpdateHandler<BotExampleContext>
    {
        private readonly ILogger<PublicChatEcho> _logger;
        private readonly IMessageReader<PublicChatEcho, BotExampleContext> _messageReader;

        public PublicChatEcho(ILogger<PublicChatEcho> logger, IMessageReader<PublicChatEcho, BotExampleContext> messageReader)
        {
            _messageReader = messageReader;
            _logger = logger;
        }

        public async Task HandleAsync(BotExampleContext context, UpdateDelegate<BotExampleContext> next, CancellationToken cancellationToken)
        {
            await context.Client.SendTextMessageAsync(context.Update.GetSenderId(), _messageReader.GetMessage("greetings"));
            _logger.LogInformation("This update {0} was from public chat {1}", context.Update.Id, context.Update.GetChat());
        }
    }
}