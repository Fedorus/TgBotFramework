using System;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgBotFramework;

namespace EchoBotProject
{
    public class BotExampleContext : BaseUpdateContext
    {
        public UserPrivateInfo Info { get; set; }
    }

    public class UserPrivateInfo
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
    }
}