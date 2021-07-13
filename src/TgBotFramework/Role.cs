using System;

namespace TgBotFramework
{
    [Flags]
    public enum Role
    {
        None = 0,
        Owner = 1,
        Admin = 2,
        Moderator= 4,
        PremiumUser = 8,
        User = 16,
        Restricted = 32,
        Banned = 64
    }
}