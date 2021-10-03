namespace TehGM.Discord.Interactions.CommandsHandling
{
    public interface IDiscordInteractionCommandsProvider
    {
        IDiscordInteractionCommand GetRegisteredCommand(ulong commandID);
        void RegisterCommand(ulong commandID, IDiscordInteractionCommand handler);
    }
}
