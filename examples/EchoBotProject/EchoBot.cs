using System;
using Microsoft.Extensions.Options;
using TgBotFramework;

namespace EchoBotProject
{
    public class EchoBot : BaseBot
    {
        public EchoBot(IOptions<BotSettings> options) : base(options)
        {
        }
    }
}