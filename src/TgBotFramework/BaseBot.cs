using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;

namespace TgBotFramework
{
    public class BaseBot 
    {
        public ITelegramBotClient Client { get; }

        public BaseBot(IOptions<BotSettings> options)
        {
            Client = new TelegramBotClient(options.Value.ApiToken, baseUrl: options.Value.BaseUrl);
        }
    }
}