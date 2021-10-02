namespace TehGM.Discord.Interactions
{
    public enum DiscordApplicationCommandType
    {
        /// <summary>User typed a slash command.</summary>
        ChatInput = 1,
        /// <summary>User right clicked on a user.</summary>
        User = 2,
        /// <summary>User right clicked on a message.</summary>
        Message = 3
    }
}
