using System;

namespace TgBotFramework.StageManaging
{
    public static class GuidFromIds
    {
        public static Guid Get(long userId, long chatId = 0)
        {
            byte[] guidData = new byte[16];
            Array.Copy(BitConverter.GetBytes(userId), guidData, 8);
            Array.Copy(BitConverter.GetBytes(chatId), 0, guidData, 8, 8);
            return new Guid(guidData);
        }
    }
}