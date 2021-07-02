namespace TgBotFramework
{
    public class UserState
    {
        public string Stage { get; set; } = "default";
        public int Step { get; set; }
        public Role Role { get; set; }
    }
}