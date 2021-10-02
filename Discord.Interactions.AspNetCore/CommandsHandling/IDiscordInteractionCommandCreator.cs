using System;

namespace TehGM.Discord.Interactions.CommandsHandling
{
    /// <summary>Represents a class that can create a complex Discord Application Command.</summary>
    /// <remarks>This interface is meant to be used for Interaction Commands that require more complex setup than attributes provide.</remarks>
    public interface IDiscordInteractionCommandCreator
    {
        /// <summary>Builds an interaction command.</summary>
        /// <param name="services">Service provider that can be used to retrieve services required for command building.</param>
        /// <returns>The built interaction command.</returns>
        DiscordApplicationCommand Build(IServiceProvider services);
    }
}
