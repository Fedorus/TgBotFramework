using TgBotFramework;

namespace EchoBotProject.Data
{
    public class UserModel
    {
        public long Id { get; set; }
        public string Stage { get; set; } 
        public long Step { get; set; }
        public Role Role { get; set; }
        public string LanguageCode { get; set; }
    }
}