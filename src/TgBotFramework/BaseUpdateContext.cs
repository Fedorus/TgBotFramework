using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TgBotFramework
{
    public class BaseUpdateContext : IUpdateContext
    {
        public Update Update { get; set; }
        public HttpContent HttpContent { get; set; }
        public IServiceProvider Services { get; set; }
        public TaskCompletionSource Result { get; set; }
        public UserState UserState { get; set; } = new UserState();
        public BaseBot Bot { get; set; }
        public TelegramBotClient Client { get; set; }
    }
}