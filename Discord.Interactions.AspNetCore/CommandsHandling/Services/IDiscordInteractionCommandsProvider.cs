namespace TehGM.Discord.Interactions.CommandsHandling
{
    /// <summary>Maps command IDs to their code.</summary>
    public interface IDiscordInteractionCommandsProvider
    {
        /// <summary>Gets a previously added command.</summary>
        /// <param name="commandID">ID of the command. Must match Discord's command ID.</param>
        /// <returns>Command instance.</returns>
        IDiscordInteractionCommand GetCommand(ulong commandID);
        /// <summary>Adds a new command.</summary>
        /// <param name="commandID">ID of the command. Must match Discord's command ID.</param>
        /// <param name="handler">The command instance.</param>
        void AddCommand(ulong commandID, IDiscordInteractionCommand handler);
    }
}
