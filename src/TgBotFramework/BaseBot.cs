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

        public BaseBot(string token, string username)
        {
            Client = new TelegramBotClient(token);
            Username = username;
        }

        public bool CanHandleCommand(string commandName, Message message)
        {
            if (string.IsNullOrWhiteSpace(commandName))
                throw new ArgumentException("Invalid command name", nameof(commandName));
            if (commandName.StartsWith("/"))
                throw new ArgumentException("Command name must not start with '/'.", nameof(commandName));

            if (message == null)
                return false;

            if (message.Text != null && message.Entities is { Length: > 0 })
                return message.Entities[0].Type == MessageEntityType.BotCommand && message.Entities[0].Offset == 0 && Regex.IsMatch(
                    message.Text.Substring(message.Entities[0].Offset, message.Entities[0].Length),
                    $@"^/{commandName}(?:@{Username})?$",
                    RegexOptions.IgnoreCase);
            
            if (message.Caption != null && message.CaptionEntities is { Length: > 0 })
                return message.CaptionEntities[0].Type == MessageEntityType.BotCommand && message.CaptionEntities[0].Offset == 0 &&
                       Regex.IsMatch(
                        message.Caption.Substring(message.CaptionEntities[0].Offset, message.CaptionEntities[0].Length),
                        $@"^/{commandName}(?:@{Username})?$",
                        RegexOptions.IgnoreCase);

            return false;
        }
    }
}