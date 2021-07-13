using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        public TelegramBotClient Client { get; }
        public string Username { get; }

        public BaseBot(IOptions<BotSettings> options)
        {
            Client = new TelegramBotClient(options.Value.ApiToken, baseUrl: options.Value.BaseUrl);
            Username = options.Value.Username;
        }
        
        public bool CanHandleCommand(string commandName, Message message)
        {
            if (string.IsNullOrWhiteSpace(commandName))
                throw new ArgumentException("Invalid command name", nameof(commandName));
            if (commandName.StartsWith("/"))
                throw new ArgumentException("Command name must not start with '/'.", nameof(commandName));

            if (message is null)
                return false;

            {
                bool isTextMessage = message.Text != null;
                if (!isTextMessage)
                    return false;
            }

            {
                bool isCommand = message.Entities?.FirstOrDefault()?.Type == MessageEntityType.BotCommand;
                if (!isCommand)
                    return false;
            }

            return Regex.IsMatch(
                message.EntityValues.First(),
                $@"^/{commandName}(?:@{Username})?$",
                RegexOptions.IgnoreCase
            );
        }
        
        
    }
}