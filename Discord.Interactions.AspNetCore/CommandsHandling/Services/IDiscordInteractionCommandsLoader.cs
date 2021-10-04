using System.Collections.Generic;
using System.Reflection;

namespace TehGM.Discord.Interactions.CommandsHandling
{
    /// <summary>A service that can load <see cref="IDiscordInteractionCommandsLoader"/> from assemblies.</summary>
    public interface IDiscordInteractionCommandsLoader
    {
        /// <summary>Loads all <see cref="IDiscordInteractionCommand"/> from an assembly.</summary>
        /// <param name="assembly">Assembly to load commands from.</param>
        /// <param name="cancellationToken">Token that can be used to stop loading.</param>
        /// <returns>Enumerable of all loaded commands.</returns>
        IEnumerable<IDiscordInteractionCommand> LoadFromAssembly(Assembly assembly);
        /// <summary>Loads <see cref="IDiscordInteractionCommand"/> from a type.</summary>
        /// <param name="type">Type to load.</param>
        /// <param name="cancellationToken">Token that can be used to stop loading.</param>
        /// <returns>Loaded command.</returns>
        IDiscordInteractionCommand LoadFromType(TypeInfo type);
    }
}
