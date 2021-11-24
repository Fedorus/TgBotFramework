namespace TgBotFramework.Data.EF.Models
{
    public class ChatMemberStatus
    {
        public int Id { get; set; }
        public virtual GroupChat GroupChat { get; set; }
        public virtual User User { get; set; }
        public Status Status { get; set; } 
    }

    public enum Status
    {
        Member,
        Left,
        Banned
    }
}