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
        /// <summary>Removes a specific command from the provider.</summary>
        /// <param name="commandID">ID of the command</param>
        /// <returns>True if the command was found and removed; otherwise false.</returns>
        bool RemoveCommand(ulong commandID);
        /// <summary>Removes all commands from the provider.</summary>
        void Clear();
    }
}
