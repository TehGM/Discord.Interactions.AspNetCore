using System;

namespace TehGM.Discord.Interactions.CommandsHandling
{
    /// <summary>Represents a class that can create a complex Discord Application Command.</summary>
    /// <remarks>This interface is meant to be used for Interaction Commands that require more complex setup than attributes provide.</remarks>
    public interface IBuildableDiscordInteractionCommand
    {
        /// <summary>Builds an application command.</summary>
        /// <param name="services">Service provider that can be used to retrieve services required for command building.</param>
        /// <returns>The built application command.</returns>
        DiscordApplicationCommand Build(IServiceProvider services);
    }
}
