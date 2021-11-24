using System.ComponentModel.DataAnnotations.Schema;

namespace TgBotFramework.Data.EF.Models
{
    public class User
    {
        public long UserId { get; set; }
        
        [ForeignKey("StateId")]
        public virtual State State { get; set; }
        
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LanguageCode { get; set; }
        public bool IsBot { get; set; }
        
        public PrivateChatStatus PrivateChatStatus { get; set; }
    }

    public enum PrivateChatStatus
    {
        NotStarted,
        Active,
        Blocked
    }
}