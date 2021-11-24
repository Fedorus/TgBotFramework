using System;
using System.ComponentModel.DataAnnotations.Schema;
using Telegram.Bot.Types.Enums;

namespace TgBotFramework.Data.EF.Models
{
    public class Update
    {
        public long UpdateId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("ChatId")]
        public virtual GroupChat GroupChat { get; set; }
        
        public UpdateType Type { get; set; }
        
        public DateTime DateTime { get; set; }

        public string UpdateObject { get; set; }
    }
}