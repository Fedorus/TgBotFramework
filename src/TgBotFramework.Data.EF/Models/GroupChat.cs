using System.Collections.Generic;
using Telegram.Bot.Types.Enums;

namespace TgBotFramework.Data.EF.Models
{
    public class GroupChat
    {
        public long GroupChatId { get; set; }
        public ICollection<User> Users { get; set; }
        public string Name { get; set; }
        public ChatType ChatType { get; set; }
    }
}