namespace TehGM.Discord.Interactions.CommandsHandling
{
    /// <summary>Represents a service that can create a <see cref="DiscordApplicationCommand"/> from <see cref="IDiscordInteractionCommand"/>.</summary>
    public interface IDiscordApplicationCommandsCreator
    {
        /// <summary>Creates a Discord Application Command.</summary>
        /// <param name="command"></param>
        /// <returns>Created Discord Application Command.</returns>
        DiscordApplicationCommand Create(IDiscordInteractionCommand command);
    }
}
