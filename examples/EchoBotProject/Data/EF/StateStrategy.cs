namespace EchoBotProject.Data.EF
{
    public enum StateStrategy
    {
        /// <summary>
        /// Each user has its own state, same for different chats
        /// </summary>
        PerUser,
        /// <summary>
        /// Each chat has its own state, each user in chat has the same state as chat (except language)
        /// </summary>
        PerChat,
        /// <summary>
        /// Each user has different state in each chat
        /// </summary>
        PerUserPerChat
    }
}