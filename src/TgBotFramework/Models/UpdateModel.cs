using Telegram.Bot.Requests;

namespace TgBotFramework.Models
{
    public class UpdateModel
    {
        public long UpdateId { get; set; }
        public long? UserId { get; set; }
        public long? ChatId { get; set; }
    }
}