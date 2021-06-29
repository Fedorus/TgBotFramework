using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace TgBotFramework
{
    public class BaseUpdateContext : IUpdateContext
    {
        public Update Update { get; set; }
        public HttpContent HttpContent { get; set; }
        public IServiceProvider Services { get; set; }
        public TaskCompletionSource Result { get; set; }
    }

    public interface IUpdateContext
    {
        public Update Update { get; set; }
        public HttpContent HttpContent { get; set; }
        public IServiceProvider Services { get; set; }
        public TaskCompletionSource Result { get; set; }
    }
}