using Telegram.Bot.Types.Enums;

namespace TgBotFramework
{
    public class LongPollingOptions
    {
        /// <summary>
        /// Timeout in seconds for long polling. Defaults to 0, i.e. usual short polling. Should be positive, short polling should be used for testing purposes only.
        /// </summary>
        public int Timeout { get; set; } = 1;
        
        /// <summary>
        /// Array of the update types you want your bot to receive.
        /// </summary>
        public UpdateType[] AllowedUpdates { get; set; }

        public bool WaitForResult { get; set; } = false;
    }
}