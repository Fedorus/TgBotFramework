namespace TgBotFramework.Models
{
    public class UserModel
    {
        public long Id { get; set; }
        public string Stage { get; set; } 
        public int Step { get; set; }
        public Role Role { get; set; }
        public string LanguageCode { get; set; }
    }
}